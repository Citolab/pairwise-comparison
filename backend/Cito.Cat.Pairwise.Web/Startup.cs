using System;
using Cito.Cat.Algorithms.Pairwise;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Interfaces;
using Cito.Cat.Core.Models;
using Cito.Cat.Pairwise.Web.Helpers;
using Cito.Cat.Service.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.DependencyInjection;

namespace Cito.Cat.Pairwise.Web
{
    public static class BackofficeSettings
    {
        public static string Environment;
    }

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostingEnvironment)
        {
            BackofficeSettings.Environment = hostingEnvironment.EnvironmentName;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddAuth(services);
            AddControllerStuff(services);
            AddPersistence(services);
            AddBusinessLogic(services);
        }

        private void AddBusinessLogic(IServiceCollection services)
        {
            services.AddSingleton<IGoodTime, GoodTime>();
            var catOptions = new CatOptions();
            Configuration.GetSection(CatOptions.Cat).Bind(catOptions);
            services.AddSingleton(catOptions);
            //services.AddScoped<ICatProcessor, PairwiseCatProcessor>();
            services.AddScoped<CatSectionHandler>();
            services.AddScoped<CatSessionHandler>();
            services.AddScoped<NextItemsHandler>();
        }

        private void AddPersistence(IServiceCollection services)
        {
            // RavenDB 3.5.9 config
            // var settings = new Settings();
            // Configuration.Bind(settings);
            //
            // var store = new DocumentStore
            // {
            //     ApiKey = settings.RavenDbSettings.ApiKey,
            //     Url = settings.RavenDbSettings.Url,
            //     DefaultDatabase = settings.RavenDbSettings.Database
            //     //Certificate = new X509Certificate2( settings.CertPath, settings.CertPass)
            // };
            //
            // store.Initialize();
            //
            // services.AddSingleton<IDocumentStore>(store);
            //
            // services.AddScoped(serviceProvider => serviceProvider
            //     .GetService<IDocumentStore>()
            //     .OpenAsyncSession());

            // ravendb 5.0.0
            // 1. Add an IDocumentStore singleton. Make sure that RavenSettings section exist in appsettings.json
            services.AddRavenDbDocStore();

            // 2. Add a scoped IAsyncDocumentSession. For the sync version, use .AddRavenSession().
            services.AddRavenDbAsyncSession();
        }

        private void AddControllerStuff(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add<DomainExceptionFilter>();
                    options.Filters.Add<RavenSaveChangesPageFilter>();
                })
                .AddRazorPagesOptions(options => { options.Conventions.AuthorizeFolder("/Admin", "AdminPolicy"); });
            services.AddResponseCompression();

            // logging
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        private static void AddAuth(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy",
                    new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser().RequireRole("Admin").Build());
                options.DefaultPolicy =
                    new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser().Build();
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options =>
                {
                    options.Cookie.Name = "catnip";
                    options.LoginPath = new PathString("/Index");
                    options.AccessDeniedPath = new PathString("/Admin/Index");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var setup = serviceProvider.GetRequiredService<IOptions<RavenOptions>>().Value;

            using var documentStore =
                setup.GetDocumentStore(docStore => docStore.Conventions.MaxNumberOfRequestsPerSession = 1000);
            using var documentSession = documentStore.OpenAsyncSession();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var catOptions = serviceProvider.GetService<CatOptions>();
            var catSectionHandler = new CatSectionHandler(new GoodTime(), catOptions, documentSession, loggerFactory);

            var seeder = new Seeder(documentSession, catSectionHandler, env.ContentRootPath, loggerFactory);
            seeder.SeedCatSectionsV2().Wait();
        }
    }
}
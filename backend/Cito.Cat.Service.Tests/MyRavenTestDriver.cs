using System.Diagnostics.CodeAnalysis;
using Cito.Cat.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.TestDriver;

namespace Cito.Cat.Service.Tests
{
    [ExcludeFromCodeCoverage]
    public class MyRavenTestDriver : RavenTestDriver
    {
        protected ILogger Logger;
        protected IDocumentStore DocumentStore;
        protected IAsyncDocumentSession DocumentSession;
        protected CatOptions CatOptions;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(config).AddConsole());
            Logger = loggerFactory.CreateLogger("Tests");

            // RavenDB.TestDriver depends on asp.net core runtime v3.1.7:
            // https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.7-windows-x64-installer
            DocumentStore = GetDocumentStore();
            
            
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DocumentStore.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            DocumentSession = DocumentStore.OpenAsyncSession();
        }

        [TearDown]
        public void TearDown()
        {
            DocumentSession.Dispose();
        }

        protected void WaitForIndexing()
        {
            WaitForIndexing(DocumentStore);
        }

        protected void WaitForUserToContinueTheTest()
        {
            WaitForUserToContinueTheTest(DocumentStore);
        }
    }
}
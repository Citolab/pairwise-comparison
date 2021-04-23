using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Helpers
{
    /// <summary>
    /// Razor Pages filter that saves any changes after the action completes.
    /// </summary>
    public class RavenSaveChangesPageFilter : IAsyncPageFilter
    {
        private readonly IAsyncDocumentSession _documentSession;

        public RavenSaveChangesPageFilter(IAsyncDocumentSession dbSession)
        {
            this._documentSession = dbSession;
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var result = await next.Invoke();

            // If there was no exception, and the action wasn't cancelled, save changes.
            if (result.Exception == null && !result.Canceled)
            {
                await this._documentSession.SaveChangesAsync();
            }
        }
    }
}
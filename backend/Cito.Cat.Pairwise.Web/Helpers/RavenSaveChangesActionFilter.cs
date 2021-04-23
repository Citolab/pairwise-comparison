using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Helpers
{
    public class RavenSaveChangesActionFilter : IAsyncActionFilter
    {
        private readonly IAsyncDocumentSession _documentSession;

        public RavenSaveChangesActionFilter(IAsyncDocumentSession dbSession)
        {
            this._documentSession = dbSession;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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
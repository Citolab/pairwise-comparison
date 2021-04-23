using System.Threading.Tasks;

namespace Cito.Cat.Pairwise.Web.Requests
{
    /// <summary>
    /// A query request.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class Query<TResponse> : Request
    {
        public Task<TResponse> Execute()
        {
            return DoExecute();
        }

        protected abstract Task<TResponse> DoExecute();
    }
}
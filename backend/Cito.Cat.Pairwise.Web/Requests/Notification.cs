using System.Threading.Tasks;

namespace Cito.Cat.Pairwise.Web.Requests
{
    /// <summary>
    /// A command request that doesn't have a return value e.g. fire-and-forget.
    /// </summary>
    public abstract class Notification : Request
    {
        public Task Execute()
        {
           
            return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}
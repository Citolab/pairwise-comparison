using System.Threading.Tasks;
using Cito.Cat.Pairwise.Web.Models;

namespace Cito.Cat.Pairwise.Web.Requests
{
    public class LoginCommand : Command<LoginResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        protected override Task<LoginResult> DoExecute()
        {
            var result = new LoginResult {Success = false};
            if (Username == "admin" && Password == "pairwiseadmin!DH@(#*KLJD*28283#BNBDKS*kdks(*&")
            {
                result = new LoginResult
                {
                    Success = true,
                    UserId = "admin",
                    FullName = "Awesome Admin",
                    UserRole = UserRole.Admin
                };
            }

            return Task.FromResult(result);
        }
    }
}
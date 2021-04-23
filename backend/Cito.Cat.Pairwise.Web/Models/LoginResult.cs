namespace Cito.Cat.Pairwise.Web.Models
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public UserRole UserRole { get; set; }
    }
}
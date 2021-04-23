using System;
using System.Linq;
using System.Security.Claims;
using Cito.Cat.Pairwise.Web.Models;

namespace Cito.Cat.Pairwise.Web.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get the id of the started test session <see cref="TestSession"/> from the <see cref="ClaimTypes.NameIdentifier"/> <see cref="Claim"/>.
        /// </summary>
        /// <param name="claimsPrincipal">The logged in user/testsession.</param>
        /// <returns>The guid of the test session if the claim exists, otherwise an empty string</returns>
        public static string GetTestSessionId(this ClaimsPrincipal claimsPrincipal)
        {
            var idClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim != null)
            {
                return idClaim.Value;
            }

            return string.Empty;
        }

        public static UserRole GetUserRole(this ClaimsPrincipal claimsPrincipal)
        {
            var roleClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim != null)
            {
                var roleString = roleClaim.Value;
                if (Enum.TryParse(roleString, out UserRole userRole))
                {
                    return userRole;
                }
            }

            return UserRole.Candidate;
        }
    }
}
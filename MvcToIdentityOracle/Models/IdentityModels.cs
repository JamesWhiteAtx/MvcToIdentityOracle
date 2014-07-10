using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Asp.Identity.Oracle;

namespace MvcToIdentityOracle.Models
{
    public static class DbFirstContextService
    {
        public static IdentityDbContext CreateDbContext()
        {
            return new IdentityDbContext();
        }

        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

    }
}
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Asp.Identity.Oracle;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using MvcToIdentityOracle.Models;

namespace MvcToIdentityOracle
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public static class PasswordPolicy
    {
        public const int RequiredLength = 4;
        public const bool RequireNonLetterOrDigit = false;
        public const bool RequireDigit = false;
        public const bool RequireLowercase = false;
        public const bool RequireUppercase = false;

        public static PasswordValidator MakePasswordValidator()
        {
            return new PasswordValidator
            {
                RequiredLength = RequiredLength,
                RequireNonLetterOrDigit = RequireNonLetterOrDigit,
                RequireDigit = RequireDigit,
                RequireLowercase = RequireLowercase,
                RequireUppercase = RequireUppercase,
            };
        }
    }

    public class ApplicationUserManager : UserManager<IdentityUser>
    {
        public ApplicationUserManager(IUserStore<IdentityUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<IdentityUser>(context.Get<IdentityDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<IdentityUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = PasswordPolicy.MakePasswordValidator();
            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 6,
            //    RequireNonLetterOrDigit = true,
            //    RequireDigit = true,
            //    RequireLowercase = true,
            //    RequireUppercase = true,
            //};

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<IdentityUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<IdentityUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<IdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public virtual Task<IdentityResult> AddToRolesAsync(string userId, params string[] roles)
        {
            IdentityResult result;
            foreach (var role in roles)
            {
                result = this.AddToRoleAsync(userId, role).Result;
                if (!result.Succeeded)
                {
                    return Task.FromResult(result);
                }
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public virtual Task<IdentityResult> RemoveFromRolesAsync(string userId, params string[] roles)
        {
            IdentityResult result;
            foreach (var role in roles)
            {
                result = this.RemoveFromRoleAsync(userId, role).Result;
                if (!result.Succeeded)
                {
                    return Task.FromResult(result);
                }
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore(context.Get<IdentityDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
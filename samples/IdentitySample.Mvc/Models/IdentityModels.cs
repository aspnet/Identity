using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;

namespace IdentitySample.Models
{
    public class ApplicationUser : IdentityUser { }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> { }

    public class IdentityDbContextOptions : DbContextOptions
    {
        public string DefaultAdminUserName { get; set; }

        public string DefaultAdminPassword { get; set; }
    }

    public static class ApplicationErrors
    {
        public static string GetDescription(IdentityFailure failure)
        {
            switch (failure)
            {
                case IdentityFailure.DuplicateUserName:
                case IdentityFailure.DuplicateEmail:
                case IdentityFailure.DuplicateRoleName:
                case IdentityFailure.UserValidationFailed:
                case IdentityFailure.UserNameTooShort:
                case IdentityFailure.RoleNameTooShort:
                case IdentityFailure.UserNameInvalid:
                case IdentityFailure.UserAlreadyInRole:
                case IdentityFailure.UserAlreadyHasPassword:
                case IdentityFailure.LoginAlreadyAssociated:
                case IdentityFailure.UserNotInRole:
                case IdentityFailure.LockoutForUserNotEnabled:
                case IdentityFailure.RoleValidationFailed:
                case IdentityFailure.InvalidEmail:
                case IdentityFailure.InvalidToken:
                case IdentityFailure.PasswordMismatch:
                case IdentityFailure.PasswordRequiresDigit:
                case IdentityFailure.PasswordRequiresLower:
                case IdentityFailure.PasswordRequiresUpper:
                case IdentityFailure.PasswordTooShort:
                case IdentityFailure.PasswordRequiresNonLetterAndDigit:
                case IdentityFailure.Unknown:
                default:
                    return failure.ToString();
            }
        }
    }
}
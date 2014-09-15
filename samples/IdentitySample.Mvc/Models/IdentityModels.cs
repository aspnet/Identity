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

    //public class Message { }

    //public class IdentityMessage : Message {
    //    public readonly string DuplicateUserName = nameof(DuplicateUserName);
    //}

    //public interface IMessageDescriber<TMessage>
    //{
    //    string Describe(TMessage message);
    //}


    public class ApplicationStrings {
        public string Describe(ManageController.ManageMessageId message)
        {
            return message == ManageController.ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
            : message == ManageController.ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            : message == ManageController.ManageMessageId.SetTwoFactorSuccess ? "Your two factor provider has been set."
            : message == ManageController.ManageMessageId.Error ? "An error has occurred."
            : message == ManageController.ManageMessageId.AddPhoneSuccess ? "The phone number was added."
            : message == ManageController.ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
            : "";
        }
    }

    public class ApplicationFailureDescriber : IdentityFailureDescriber
    {
        // Should we just scaffold the errors used by default?
        public override string Describe(IdentityFailure failure)
        {
            // Option 1:
            //return failure == IdentityFailure.DuplicateUserName ? "UserName is already taken."
            //    : failure == IdentityFailure.DuplicateEmail ? "Email is already taken."
            //    : failure.ToString();

            switch (failure)
            {
                case IdentityFailure.DuplicateUserName:
                    return "UserName is already taken.";
                case IdentityFailure.DuplicateEmail:
                    return "Email is already taken.";
                case IdentityFailure.DuplicateRoleName:
                    return "Role Name is already taken.";
                case IdentityFailure.UserValidationFailed:
                    return "User validation failed.";
                case IdentityFailure.UserNameTooShort:
                    return "UserName is too short.";
                case IdentityFailure.RoleNameTooShort:
                    return "Role Name is too short.";
                case IdentityFailure.UserNameHasInvalidCharacters:
                    return "UserName contains invalid characters.";
                case IdentityFailure.UserAlreadyInRole:
                    return "User is already in the role.";
                case IdentityFailure.UserAlreadyHasPassword:
                    return "User already has a password.";
                case IdentityFailure.LoginAlreadyAssociated:
                    return "Login already associated with another user.";
                case IdentityFailure.UserNotInRole:
                    return "User not in role.";
                case IdentityFailure.LockoutForUserNotEnabled:
                    return "Lockout not enabled for user.";
                case IdentityFailure.RoleValidationFailed:
                    return "Role validation failed.";
                case IdentityFailure.InvalidEmail:
                    return "Email is invalid.";
                case IdentityFailure.InvalidToken:
                    return "Token is invalid.";
                case IdentityFailure.PasswordMismatch:
                    return "Invalid username or password.";
                case IdentityFailure.PasswordRequiresDigit:
                    return "Password requires a digit [0-9].";
                case IdentityFailure.PasswordRequiresLower:
                    return "Password requires a lower case letter [a-z].";
                case IdentityFailure.PasswordRequiresUpper:
                    return "Password requires an upper case letter [A-Z].";
                case IdentityFailure.PasswordTooShort:
                    return "Password too short.";
                case IdentityFailure.PasswordRequiresNonLetterAndDigit:
                    return "Password requires a non letter and non digit character.";
                case IdentityFailure.Unknown:
                    return "Unknown error.";
                default:
                    return failure.ToString();
            }
        }
    }
}
using System;
using System.Security.Claims;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Configuration for lockout
    /// </summary>
    public class IdentityOptions
    {
        /// <summary>
        ///     ClaimType used for the security stamp by default
        /// </summary>
        public static readonly string DefaultSecurityStampClaimType = "AspNet.Identity.SecurityStamp";

        public IdentityOptions()
        {
            // TODO: Split into sub options
            ClaimTypeRole = ClaimTypes.Role;
            ClaimTypeSecurityStamp = DefaultSecurityStampClaimType;
            ClaimTypeUserId = ClaimTypes.NameIdentifier;
            ClaimTypeUserName = ClaimTypes.Name;

            UsersAllowOnlyAlphanumericNames = true;
            //UsersRequireUniqueEmail = true; // TODO: app decision?

            PasswordsRequireDigit = true;
            PasswordsRequireLowercase = true;
            PasswordsRequireNonLetterOrDigit = true;
            PasswordsRequireUppercase = true;
            PasswordsRequiredLength = 6;

            LockoutDefaultTimeSpan = TimeSpan.FromMinutes(5);
            LockoutEnabledByDefault = false;
            LockoutMaxFailedAccessAttempts = 5;
        }

        /// <summary>
        ///     Claim type used for role claims
        /// </summary>
        public string ClaimTypeRole { get; set; }

        /// <summary>
        ///     Claim type used for the user name
        /// </summary>
        public string ClaimTypeUserName { get; set; }

        /// <summary>
        ///     Claim type used for the user id
        /// </summary>
        public string ClaimTypeUserId { get; set; }

        /// <summary>
        ///     Claim type used for the user security stamp
        /// </summary>
        public string ClaimTypeSecurityStamp { get; set; }

        /// <summary>
        ///     Only allow [A-Za-z0-9@_] in UserNames
        /// </summary>
        public bool UsersAllowOnlyAlphanumericNames { get; set; }

        /// <summary>
        ///     If set, enforces that emails are non empty, valid, and unique
        /// </summary>
        public bool UsersRequireUniqueEmail { get; set; }

        /// <summary>
        ///     Minimum required length
        /// </summary>
        public int PasswordsRequiredLength { get; set; }

        /// <summary>
        ///     Require a non letter or digit character
        /// </summary>
        public bool PasswordsRequireNonLetterOrDigit { get; set; }

        /// <summary>
        ///     Require a lower case letter ('a' - 'z')
        /// </summary>
        public bool PasswordsRequireLowercase { get; set; }

        /// <summary>
        ///     Require an upper case letter ('A' - 'Z')
        /// </summary>
        public bool PasswordsRequireUppercase { get; set; }

        /// <summary>
        ///     Require a digit ('0' - '9')
        /// </summary>
        public bool PasswordsRequireDigit { get; set; }

        /// <summary>
        ///     If true, will enable user lockout when users are created
        /// </summary>
        public bool LockoutEnabledByDefault { get; set; }

        /// <summary>
        ///     Number of access attempts allowed for a user before lockout (if enabled)
        /// </summary>
        public int LockoutMaxFailedAccessAttempts { get; set; }

        /// <summary>
        ///     Default amount of time an user is locked out for after MaxFailedAccessAttempsBeforeLockout is reached
        /// </summary>
        public TimeSpan LockoutDefaultTimeSpan { get; set; }

        public void Copy(IdentityOptions options)
        {
            if (options == null)
            {
                return;
            }
            ClaimTypeRole = options.ClaimTypeRole;
            ClaimTypeSecurityStamp = 
                ClaimTypeSecurityStamp;
            ClaimTypeUserId = options.ClaimTypeUserId;
            ClaimTypeUserName = options.ClaimTypeUserName;

            UsersAllowOnlyAlphanumericNames = options.UsersAllowOnlyAlphanumericNames;
            UsersRequireUniqueEmail = options.UsersRequireUniqueEmail;

            PasswordsRequireDigit = options.PasswordsRequireDigit;
            PasswordsRequireLowercase = options.PasswordsRequireLowercase;
            PasswordsRequireNonLetterOrDigit = options.PasswordsRequireNonLetterOrDigit;
            PasswordsRequireUppercase = options.PasswordsRequireUppercase;
            PasswordsRequiredLength = options.PasswordsRequiredLength;

            LockoutDefaultTimeSpan = options.LockoutDefaultTimeSpan;
            LockoutEnabledByDefault = options.LockoutEnabledByDefault;
            LockoutMaxFailedAccessAttempts = options.LockoutMaxFailedAccessAttempts;
        }
    }
}
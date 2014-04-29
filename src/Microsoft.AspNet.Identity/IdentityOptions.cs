using System;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Microsoft.AspNet.Identity
{
    public class ClaimTypeOptions
    {
        /// <summary>
        ///     ClaimType used for the security stamp by default
        /// </summary>
        public static readonly string DefaultSecurityStampClaimType = "AspNet.Identity.SecurityStamp";

        public ClaimTypeOptions()
        {
            Role = ClaimTypes.Role;
            SecurityStamp = DefaultSecurityStampClaimType;
            UserId = ClaimTypes.NameIdentifier;
            UserName = ClaimTypes.Name;
            
        }

        /// <summary>
        ///     Claim type used for role claims
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        ///     Claim type used for the user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Claim type used for the user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Claim type used for the user security stamp
        /// </summary>
        public string SecurityStamp { get; set; }

        public virtual void Copy(ClaimTypeOptions options)
        {
            if (options == null)
            {
                return;
            }
            Role = options.Role;
            SecurityStamp = options.SecurityStamp;
            UserId = options.UserId;
            UserName = options.UserName;
        }
    }

    public class UserOptions
    {
        public UserOptions()
        {
            AllowOnlyAlphanumericNames = true;
            //User.RequireUniqueEmail = true; // TODO: app decision?
        }

        /// <summary>
        ///     Only allow [A-Za-z0-9@_] in UserNames
        /// </summary>
        public bool AllowOnlyAlphanumericNames { get; set; }

        /// <summary>
        ///     If set, enforces that emails are non empty, valid, and unique
        /// </summary>
        public bool RequireUniqueEmail { get; set; }

        public virtual void Copy(UserOptions options)
        {
            if (options == null)
            {
                return;
            }
            AllowOnlyAlphanumericNames = options.AllowOnlyAlphanumericNames;
            RequireUniqueEmail = options.RequireUniqueEmail;
        }
    }

    public class PasswordOptions
    {
        public PasswordOptions()
        {
            RequireDigit = true;
            RequireLowercase = true;
            RequireNonLetterOrDigit = true;
            RequireUppercase = true;
            RequiredLength = 6;
        }

        /// <summary>
        ///     Minimum required length
        /// </summary>
        public int RequiredLength { get; set; }

        /// <summary>
        ///     Require a non letter or digit character
        /// </summary>
        public bool RequireNonLetterOrDigit { get; set; }

        /// <summary>
        ///     Require a lower case letter ('a' - 'z')
        /// </summary>
        public bool RequireLowercase { get; set; }

        /// <summary>
        ///     Require an upper case letter ('A' - 'Z')
        /// </summary>
        public bool RequireUppercase { get; set; }

        /// <summary>
        ///     Require a digit ('0' - '9')
        /// </summary>
        public bool RequireDigit { get; set; }

        public virtual void Copy(PasswordOptions options)
        {
            if (options == null)
            {
                return;
            }
            RequireDigit = options.RequireDigit;
            RequireLowercase = options.RequireLowercase;
            RequireNonLetterOrDigit = options.RequireNonLetterOrDigit;
            RequireUppercase = options.RequireUppercase;
            RequiredLength = options.RequiredLength;
        }
    }

    public class LockoutOptions
    {
        public LockoutOptions()
        {
            DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            EnabledByDefault = false;
            MaxFailedAccessAttempts = 5;
        }
        public void Copy(LockoutOptions options)
        {
            if (options == null)
            {
                return;
            }

            DefaultLockoutTimeSpan = options.DefaultLockoutTimeSpan;
            EnabledByDefault = options.EnabledByDefault;
            MaxFailedAccessAttempts = options.MaxFailedAccessAttempts;
        }

        /// <summary>
        ///     If true, will enable user lockout when users are created
        /// </summary>
        public bool EnabledByDefault { get; set; }


        /// <summary>
        ///     Number of access attempts allowed for a user before lockout (if enabled)
        /// </summary>
        public int MaxFailedAccessAttempts { get; set; }


        /// <summary>
        ///     Default amount of time an user is locked out for after MaxFailedAccessAttempsBeforeLockout is reached
        /// </summary>
        public TimeSpan DefaultLockoutTimeSpan { get; set; }
    }

    /// <summary>
    ///     Configuration for lockout
    /// </summary>
    public class IdentityOptions
    {

        public IdentityOptions()
        {
            // TODO: Split into sub options
            ClaimType = new ClaimTypeOptions();
            User = new UserOptions();
            Password = new PasswordOptions();
            Lockout = new LockoutOptions();
        }

        public ClaimTypeOptions ClaimType { get; set; }

        public UserOptions User { get; set; }

        public PasswordOptions Password { get; set; }

        public LockoutOptions Lockout { get; set; }

        public void Copy(IdentityOptions options)
        {
            if (options == null)
            {
                return;
            }
            User.Copy(options.User);
            Password.Copy(options.Password);
            Lockout.Copy(options.Lockout);
            ClaimType.Copy(options.ClaimType);
        }
    }
}
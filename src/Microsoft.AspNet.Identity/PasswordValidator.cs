using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.DependencyInjection;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Used to validate some basic password policy like length and number of non alphanumerics
    /// </summary>
    public class PasswordValidator : IPasswordValidator
    {
        public PasswordValidator(IOptionsAccessor<IdentityOptions> options)
        {
            if (options == null || options.Options == null)
            {
                throw new ArgumentNullException("options");
            }
            RequireDigit = options.Options.PasswordsRequireDigit;
            RequiredLength = options.Options.PasswordsRequiredLength;
            RequireLowercase = options.Options.PasswordsRequireLowercase;
            RequireUppercase = options.Options.PasswordsRequireUppercase;
            RequireNonLetterOrDigit = options.Options.PasswordsRequireNonLetterOrDigit;
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

        /// <summary>
        ///     Ensures that the string is of the required length and meets the configured requirements
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<IdentityResult> ValidateAsync(string item, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(item) || item.Length < RequiredLength)
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.PasswordTooShort, RequiredLength));
            }
            if (RequireNonLetterOrDigit && item.All(IsLetterOrDigit))
            {
                errors.Add(Resources.PasswordRequireNonLetterOrDigit);
            }
            if (RequireDigit && !item.Any(IsDigit))
            {
                errors.Add(Resources.PasswordRequireDigit);
            }
            if (RequireLowercase && !item.Any(IsLower))
            {
                errors.Add(Resources.PasswordRequireLower);
            }
            if (RequireUppercase && !item.Any(IsUpper))
            {
                errors.Add(Resources.PasswordRequireUpper);
            }
            return
                Task.FromResult(errors.Count == 0
                    ? IdentityResult.Success
                    : IdentityResult.Failed(String.Join(" ", errors)));
        }

        /// <summary>
        ///     Returns true if the character is a digit between '0' and '9'
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        ///     Returns true if the character is between 'a' and 'z'
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }

        /// <summary>
        ///     Returns true if the character is between 'A' and 'Z'
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        /// <summary>
        ///     Returns true if the character is upper, lower, or a digit
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual bool IsLetterOrDigit(char c)
        {
            return IsUpper(c) || IsLower(c) || IsDigit(c);
        }
    }
}
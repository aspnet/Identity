using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Used to validate some basic password policy like length and number of non alphanumerics
    /// </summary>
    public class PasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        /// <summary>
        ///     Ensures that the password is of the required length and meets the configured requirements
        /// </summary>
        /// <param name="password"></param>
        /// <param name="manager"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<IdentityResult> ValidateAsync(string password, UserManager<TUser> manager, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(password) || password.Length < manager.Options.PasswordsRequiredLength)
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.PasswordTooShort, manager.Options.PasswordsRequiredLength));
            }
            if (manager.Options.PasswordsRequireNonLetterOrDigit && password.All(IsLetterOrDigit))
            {
                errors.Add(Resources.PasswordRequireNonLetterOrDigit);
            }
            if (manager.Options.PasswordsRequireDigit && !password.Any(IsDigit))
            {
                errors.Add(Resources.PasswordRequireDigit);
            }
            if (manager.Options.PasswordsRequireLowercase && !password.Any(IsLower))
            {
                errors.Add(Resources.PasswordRequireLower);
            }
            if (manager.Options.PasswordsRequireUppercase && !password.Any(IsUpper))
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
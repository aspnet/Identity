// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        public virtual Task<IdentityResult> ValidateAsync(string password, UserManager<TUser> manager, 
            CancellationToken cancellationToken = default(CancellationToken))
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
            var options = manager.Options.Password;
            if (string.IsNullOrWhiteSpace(password) || password.Length < options.RequiredLength)
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.PasswordTooShort, 
                    options.RequiredLength));
            }
            if (options.RequireNonLetterOrDigit && password.All(IsLetterOrDigit))
            {
                errors.Add(Resources.PasswordRequireNonLetterOrDigit);
            }
            if (options.RequireDigit && !password.Any(IsDigit))
            {
                errors.Add(Resources.PasswordRequireDigit);
            }
            if (options.RequireLowercase && !password.Any(IsLower))
            {
                errors.Add(Resources.PasswordRequireLower);
            }
            if (options.RequireUppercase && !password.Any(IsUpper))
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
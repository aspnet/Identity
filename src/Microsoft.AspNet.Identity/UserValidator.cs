// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
#if ASPNET50
using System.Net.Mail;
#endif
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Validates users before they are saved
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserValidator<TUser> : IUserValidator<TUser> where TUser : class
    {
        /// <summary>
        ///     Validates a user before saving
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var errors = new List<IdentityFailure>();
            await ValidateUserName(manager, user, errors);
            if (manager.Options.User.RequireUniqueEmail)
            {
                await ValidateEmail(manager, user, errors);
            }
            return errors.Count > 0 ? new IdentityResult(errors) : IdentityResult.Success;
        }

        private async Task ValidateUserName(UserManager<TUser> manager, TUser user, ICollection<IdentityFailure> errors)
        {
            var userName = await manager.GetUserNameAsync(user);
            if (string.IsNullOrWhiteSpace(userName))
            {
                errors.Add(IdentityFailure.UserNameTooShort);
            }
            else if (manager.Options.User.UserNameValidationRegex != null && !Regex.IsMatch(userName, manager.Options.User.UserNameValidationRegex))
            {
                errors.Add(IdentityFailure.UserNameHasInvalidCharacters);
            }
            else
            {
                var owner = await manager.FindByNameAsync(userName);
                if (owner != null && 
                    !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
                {
                    errors.Add(IdentityFailure.DuplicateUserName);
                }
            }
        }

        // make sure email is not empty, valid, and unique
        private static async Task ValidateEmail(UserManager<TUser> manager, TUser user, List<IdentityFailure> errors)
        {
            var email = await manager.GetEmailAsync(user);
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(IdentityFailure.InvalidEmail);
                return;
            }
#if ASPNET50
            try
            {
                var m = new MailAddress(email);
            }
            catch (FormatException)
            {
                errors.Add(IdentityFailure.InvalidEmail);
                return;
            }
#endif
            var owner = await manager.FindByEmailAsync(email);
            if (owner != null && 
                !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
            {
                errors.Add(IdentityFailure.DuplicateEmail);
            }
        }
    }
}

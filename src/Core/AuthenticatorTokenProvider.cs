// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Used for authenticator code verification.
    /// </summary>
    public class AuthenticatorTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : class
    {
  
        /// <summary>
        /// Checks if a two factor authentication token can be generated for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve the <paramref name="user"/> from.</param>
        /// <param name="user">The <typeparamref name="TUser"/> to check for the possibility of generating a two factor authentication token.</param>
        /// <returns>True if the user has an authenticator key set, otherwise false.</returns>
        public async virtual Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            var key = await manager.GetAuthenticatorKeyAsync(user);
            return !string.IsNullOrWhiteSpace(key);
        }

        /// <summary>
        /// Returns an empty string since no authenticator codes are sent.
        /// </summary>
        /// <param name="purpose">Ignored.</param>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve the <paramref name="user"/> from.</param>
        /// <param name="user">The <typeparamref name="TUser"/>.</param>
        /// <param name="length">The length of code.</param>
        /// <returns>string.Empty.</returns>
        public virtual Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user, int length)
        {
            return Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="token"/> is valid for the given
        /// <paramref name="user"/> and <paramref name="purpose"/>.
        /// </summary>
        /// <param name="purpose">The purpose the token will be used for.</param>
        /// <param name="token">The token to validate.</param>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
        /// <param name="user">The user a token should be validated for.</param>
        /// <param name="length">The length of code.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the a flag indicating the result
        /// of validating the <paramref name="token"> for the specified </paramref><paramref name="user"/> and <paramref name="purpose"/>.
        /// The task will return true if the token is valid, otherwise false.
        /// </returns>
	public virtual async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user, int length)
        {
            var key = await manager.GetAuthenticatorKeyAsync(user);
            int code;
            if (!int.TryParse(token, out code))
            {
                return false;
            }

            var hash = new HMACSHA1(Base32.FromBase32(key));
            var unixTimestamp = Convert.ToInt64(Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
            var timestep = Convert.ToInt64(unixTimestamp / 30);
            // Allow codes from 90s in each direction (we could make this configurable?)
            for (int i = -2; i <= 2; i++)
            {
                var expectedCode = Rfc6238AuthenticationService.ComputeTotp(hash, (ulong)(timestep + i), modifier: null, length);
                if (expectedCode == code)
                {
                    return true;
                }
            }
            return false;
        }
   }
}
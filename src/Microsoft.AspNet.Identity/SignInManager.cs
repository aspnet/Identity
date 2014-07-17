// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Interface that manages SignIn operations for a user
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class SignInManager<TUser> where TUser : class
    {
        public SignInManager(UserManager<TUser> userManager, IAuthenticationManager authenticationManager, 
            IClaimsIdentityFactory<TUser> claimsFactory)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            if (authenticationManager == null)
            {
                throw new ArgumentNullException("authenticationManager");
            }
            if (claimsFactory == null)
            {
                throw new ArgumentNullException("claimsFactory");
            }
            UserManager = userManager;
            AuthenticationManager = authenticationManager;
            ClaimsFactory = claimsFactory;
        }

        public UserManager<TUser> UserManager { get; private set; }
        public IAuthenticationManager AuthenticationManager { get; private set; }
        public IClaimsIdentityFactory<TUser> ClaimsFactory { get; private set; }

        // Should this be a func?
        public virtual async Task<ClaimsIdentity> CreateUserIdentityAsync(TUser user)
        {
            // REVIEW: should sign in manager take options instead of using the user manager instance?
            return await ClaimsFactory.CreateAsync(user, UserManager.Options.ClaimsIdentity);
        }

        public virtual async Task SignInAsync(TUser user, bool isPersistent)
        {
            var userIdentity = await CreateUserIdentityAsync(user);
            AuthenticationManager.SignIn(userIdentity, isPersistent);
        }

        // TODO: Should this be async?
        public virtual void SignOut()
        {
            // REVIEW: need a new home for this option config?
            AuthenticationManager.SignOut(UserManager.Options.ClaimsIdentity.AuthenticationType);
        }

        /// <summary>
        /// Validates that the claims identity has a security stamp matching the users
        /// Returns the user if it matches, null otherwise
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<TUser> ValidateSecurityStamp(ClaimsIdentity identity, string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user != null && UserManager.SupportsUserSecurityStamp)
            {
                var securityStamp =
                    identity.FindFirstValue(UserManager.Options.ClaimsIdentity.SecurityStampClaimType);
                if (securityStamp == await UserManager.GetSecurityStampAsync(user))
                {
                    return user;
                }
            }
            return null;
        }

        public virtual async Task<SignInStatus> PasswordSignInAsync(string userName, string password, 
            bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user))
            {
                return SignInStatus.LockedOut;
            }
            if (await UserManager.CheckPasswordAsync(user, password))
            {
                return await SignInOrTwoFactor(user, isPersistent);
            }
            if (UserManager.SupportsUserLockout && shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user);
                if (await UserManager.IsLockedOutAsync(user))
                {
                    return SignInStatus.LockedOut;
                }
            }
            return SignInStatus.Failure;
        }

        public virtual async Task<bool> SendTwoFactorCode(string provider)
        {
            var userId = await AuthenticationManager.RetrieveUserId();
            if (userId == null)
            {
                return false;
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var token = await UserManager.GenerateTwoFactorTokenAsync(user, provider);
            // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
            await UserManager.NotifyTwoFactorTokenAsync(user, provider, token);
            return true;
        }

        public async Task<bool> IsTwoFactorClientRemembered(TUser user)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            return await AuthenticationManager.IsClientRememeberedAsync(userId);
        }

        public virtual async Task RememberTwoFactorClient(TUser user)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            AuthenticationManager.RememberClient(userId);
        }

        public virtual Task ForgetTwoFactorClientAsync()
        {
            AuthenticationManager.ForgetClient();
            return Task.FromResult(0);
        }

        public virtual async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent)
        {
            var userId = await AuthenticationManager.RetrieveUserId();
            if (userId == null)
            {
                return SignInStatus.Failure;
            }
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user))
            {
                return SignInStatus.LockedOut;
            }
            if (await UserManager.VerifyTwoFactorTokenAsync(user, provider, code))
            {
                // When token is verified correctly, clear the access failed count used for lockout
                await UserManager.ResetAccessFailedCountAsync(user);
                await SignInAsync(user, isPersistent);
                return SignInStatus.Success;
            }
            // If the token is incorrect, record the failure which also may cause the user to be locked out
            await UserManager.AccessFailedAsync(user);
            return SignInStatus.Failure;
        }

        public async Task<SignInStatus> ExternalLoginSignInAsync(UserLoginInfo loginInfo, bool isPersistent)
        {
            var user = await UserManager.FindByLoginAsync(loginInfo);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user))
            {
                return SignInStatus.LockedOut;
            }
            return await SignInOrTwoFactor(user, isPersistent);
        }

        private async Task<SignInStatus> SignInOrTwoFactor(TUser user, bool isPersistent)
        {
            if (UserManager.SupportsUserTwoFactor && await UserManager.GetTwoFactorEnabledAsync(user))
            {
                if (!await IsTwoFactorClientRemembered(user))
                {
                    // Store the userId for use after two factor check
                    var userId = await UserManager.GetUserIdAsync(user);
                    await AuthenticationManager.StoreUserId(userId);
                    return SignInStatus.RequiresVerification;
                }
            }
            await SignInAsync(user, isPersistent);
            return SignInStatus.Success;
        }
    }
}
// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    public interface ISignInService
    {
        void SignIn(ClaimsIdentity identity, bool isPersistent);
        void SignOut(string authenticationType);

        // remember browser for two factor
        void ForgetClient();
        void RememberClient(string userId);
        Task<bool> IsClientRememeberedAsync(string userId);

        // half cookie
        Task StoreUserId(string userId);
        Task<string> RetrieveUserId();
    }

    /// <summary>
    ///     Interface that manages SignIn operations for a user
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class SignInManager<TUser> where TUser : class
    {
        public SignInManager(UserManager<TUser> userManager, ISignInService signInService)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            if (signInService == null)
            {
                throw new ArgumentNullException("signInService");
            }
            UserManager = userManager;
            SignInService = signInService;
            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie;
        }

        // TODO: this should go into some kind of Options/setup
        public string AuthenticationType { get; set; }

        public UserManager<TUser> UserManager { get; private set; }
        public ISignInService SignInService { get; private set; }

        // Should this be a func?
        public virtual async Task<ClaimsIdentity> CreateUserIdentityAsync(TUser user)
        {
            return await UserManager.CreateIdentityAsync(user, AuthenticationType);
        }

        public virtual async Task SignInAsync(TUser user, bool isPersistent)
        {
            var userIdentity = await CreateUserIdentityAsync(user);
            SignInService.SignIn(userIdentity, isPersistent);
        }

        // TODO: Should this be async?
        public void SignOut()
        {
            SignInService.SignOut(AuthenticationType);
        }

        public virtual async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
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
            var userId = await SignInService.RetrieveUserId();
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

        //public async Task<bool> HasBeenVerified()
        //{
        //    return await GetVerifiedUserId() != null;
        //}

        public virtual async Task RememberTwoFactorClient(TUser user)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            SignInService.RememberClient(userId);
        }

        public virtual Task ForgetTwoFactorClientAsync()
        {
            SignInService.ForgetClient();
            return Task.FromResult(0);
        }

        public virtual async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent)
        {
            var userId = await SignInService.RetrieveUserId();
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
            if (await UserManager.GetTwoFactorEnabledAsync(user))
            {
                var userId = await UserManager.GetUserIdAsync(user);
                if (!await SignInService.IsClientRememeberedAsync(userId))
                {
                    // Store the userId for use after two factor check
                    await SignInService.StoreUserId(userId);
                    return SignInStatus.RequiresVerification;
                }
            }
            await SignInAsync(user, isPersistent);
            return SignInStatus.Success;
        }
    }
}
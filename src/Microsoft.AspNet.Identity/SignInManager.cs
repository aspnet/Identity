// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Interface that manages SignIn operations for a user
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public abstract class SignInManager<TManager, TUser> where TManager : UserManager<TUser> where TUser : class
    {
        public SignInManager(TManager userManager)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            UserManager = userManager;
        }

        // TODO: this should go into some kind of Options/setup
        public string AuthenticationType { get; set; }

        public TManager UserManager { get; private set; }

        public virtual async Task<ClaimsIdentity> CreateUserIdentityAsync(TUser user)
        {
            return await UserManager.CreateIdentityAsync(user, AuthenticationType);
        }

        public abstract Task SignInAsync(TUser user, bool isPersistent, bool rememberBrowser);

        // TODO: Should this be async?
        public abstract void SignOut();

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
                await SignInAsync(user, isPersistent, false);
                return SignInStatus.Success;
                //TODO: return await SignInOrTwoFactor(user, isPersistent);
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
    }
}
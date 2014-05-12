// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Security;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Identity.Security
{
    public class HttpSignInManager<TUser> : HttpSignInManager<UserManager<TUser>, TUser> where TUser : class
    {
        public HttpSignInManager(UserManager<TUser> userManager, IContextAccessor<HttpContext> contextAccessor)
            : base(userManager, contextAccessor) { }
    }


    public class HttpSignInManager<TManager, TUser> : SignInManager<TManager, TUser> where TManager : UserManager<TUser> where TUser : class
    {
        public HttpSignInManager(TManager userManager, IContextAccessor<HttpContext> contextAccessor) : base(userManager)
        {
            if (contextAccessor == null || contextAccessor.Value == null)
            {
                throw new ArgumentNullException("contextAccessor");
            }
            Context = contextAccessor.Value;
            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie;
        }

        public HttpContext Context { get; private set; }

        public override async Task SignInAsync(TUser user, bool isPersistent, bool rememberBrowser)
        {
            // TODO: all the two factor logic/external/rememberBrowser
            var userIdentity = await CreateUserIdentityAsync(user);
            Context.Response.SignIn(userIdentity, new AuthenticationProperties { IsPersistent = isPersistent });
        }

        public override void SignOut()
        {
            Context.Response.SignOut(AuthenticationType);
        }

        //public virtual async Task<bool> SendTwoFactorCode(string provider)
        //{
        //    var userId = await GetVerifiedUserId();
        //    if (userId == null)
        //    {
        //        return false;
        //    }

        //    var token = await UserManager.GenerateTwoFactorTokenAsync(userId, provider);
        //    // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
        //    await UserManager.NotifyTwoFactorTokenAsync(userId, provider, token);
        //    return true;
        //}

        //public Task<TKey> GetVerifiedUserId()
        //{
        //    //var result = await AuthenticationManager.Authenticate(DefaultAuthenticationTypes.TwoFactorCookie);
        //    //if (result != null && result.Identity != null && !String.IsNullOrEmpty(result.Identity.GetUserIdAsync()))
        //    //{
        //    //    return result.Identity.GetUserIdAsync();
        //    //}
        //    return Task.FromResult(default(TKey));
        //}

        //public async Task<bool> HasBeenVerified()
        //{
        //    return await GetVerifiedUserId() != null;
        //}

        //public virtual async Task<SignInStatus> TwoFactorSignIn(string provider, string code, bool isPersistent, bool rememberBrowser)
        //{
        //    var userId = await GetVerifiedUserId();
        //    if (userId == null)
        //    {
        //        return SignInStatus.Failure;
        //    }
        //    var user = await UserManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return SignInStatus.Failure;
        //    }
        //    if (await UserManager.IsLockedOutAsync(user.Id))
        //    {
        //        return SignInStatus.LockedOut;
        //    }
        //    if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, provider, code))
        //    {
        //        // When token is verified correctly, clear the access failed count used for lockout
        //        await UserManager.ResetAccessFailedCountAsync(user.Id);
        //        await SignIn(user, isPersistent, rememberBrowser);
        //        return SignInStatus.Success;
        //    }
        //    // If the token is incorrect, record the failure which also may cause the user to be locked out
        //    await UserManager.AccessFailedAsync(user.Id);
        //    return SignInStatus.Failure;
        //}

        //public async Task<SignInStatus> ExternalSignIn(ExternalLoginInfo loginInfo, bool isPersistent)
        //{
        //    var user = await UserManager.FindByLoginAsync(loginInfo.Login);
        //    if (user == null)
        //    {
        //        return SignInStatus.Failure;
        //    }
        //    if (await UserManager.IsLockedOutAsync(user.Id))
        //    {
        //        return SignInStatus.LockedOut;
        //    }
        //    return await SignInOrTwoFactor(user, isPersistent);
        //}

        //private async Task<SignInStatus> SignInOrTwoFactor(TUser user, bool isPersistent)
        //{
        //    if (await UserManager.GetTwoFactorEnabledAsync(user.Id))
        //        //&& !await AuthenticationManager.TwoFactorBrowserRemembered(user.Id))
        //    {
        //        //var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
        //        //identity.AddClaimAsync(new Claim(ClaimTypes.NameIdentifier, user.Id));
        //        //AuthenticationManager.SignIn(identity);
        //        return SignInStatus.RequiresTwoFactorAuthentication;
        //    }
        //    await SignIn(user, isPersistent, false);
        //    return SignInStatus.Success;
        //}
    }
}
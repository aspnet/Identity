// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Security;
using Microsoft.Framework.DependencyInjection;
using System;

namespace Microsoft.AspNet.Identity.Authentication
{
    public class HttpAuthenticationManager : IAuthenticationManager
    {
        public static readonly string TwoFactorUserIdAuthenticationType = "Microsoft.AspNet.Identity.TwoFactor.UserId";
        public static readonly string TwoFactorRememberedAuthenticationType = "Microsoft.AspNet.Identity.TwoFactor.Remembered";

        public HttpAuthenticationManager(IContextAccessor<HttpContext> contextAccessor)
        {
            Context = contextAccessor.Value;
        }

        public HttpContext Context { get; private set; }

        public virtual void ForgetClient()
        {
            Context.Response.SignOut(TwoFactorRememberedAuthenticationType);
        }

        public virtual async Task<bool> IsClientRememeberedAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result =
                await Context.AuthenticateAsync(TwoFactorRememberedAuthenticationType);
            return (result != null && result.Identity != null && result.Identity.Name == userId);
        }

        public virtual void RememberClient(string userId)
        {
            var rememberBrowserIdentity = new ClaimsIdentity(TwoFactorRememberedAuthenticationType);
            rememberBrowserIdentity.AddClaim(new Claim(ClaimTypes.Name, userId));
            Context.Response.SignIn(rememberBrowserIdentity);
        }

        public virtual async Task<TwoFactorAuthenticationInfo> RetrieveTwoFactorInfo(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await Context.AuthenticateAsync(TwoFactorUserIdAuthenticationType);
            if (result != null && result.Identity != null)
            {
                return new TwoFactorAuthenticationInfo
                {
                    UserId = result.Identity.Name,
                    LoginProvider = result.Identity.FindFirstValue(ClaimTypes.AuthenticationMethod)
                };
            }
            return null;
        }

        public virtual void SignIn(ClaimsIdentity identity, bool isPersistent)
        {
            Context.Response.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        public virtual void SignOut(string authenticationType)
        {
            Context.Response.SignOut(authenticationType);
        }

        public static ClaimsIdentity CreateIdentity(TwoFactorAuthenticationInfo info)
        {
            if (info == null)
            {
                return null;
            }
            var identity = new ClaimsIdentity(TwoFactorUserIdAuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, info.UserId));
            if (info.LoginProvider != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, info.LoginProvider));
            }
            return identity;
        }

        public virtual Task StoreTwoFactorInfo(TwoFactorAuthenticationInfo info, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            Context.Response.SignIn(CreateIdentity(info));
            return Task.FromResult(0);
        }
    }
}
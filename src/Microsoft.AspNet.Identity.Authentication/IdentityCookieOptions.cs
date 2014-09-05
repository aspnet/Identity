// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.Authentication;
using Microsoft.AspNet.Security;
using Microsoft.AspNet.Security.Cookies;
using System;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Configuration for identity
    /// </summary>
    public class IdentityCookieOptions<TUser> : IdentityOptions where TUser : class
    {
        public CookieAuthenticationOptions ApplicationCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ClaimsIdentityOptions.DefaultAuthenticationType,
            LoginPath = new PathString("/Account/Login"),
            Notifications = new CookieAuthenticationNotifications
            {
                OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<TUser>(
                        validateInterval: TimeSpan.FromMinutes(30))
            }
        };

        public string DefaultSignInAsAuthenticationType { get; set; } = ClaimsIdentityOptions.DefaultExternalLoginAuthenticationType;

        public CookieAuthenticationOptions ExternalCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ClaimsIdentityOptions.DefaultExternalLoginAuthenticationType,
            AuthenticationMode = AuthenticationMode.Passive
        };

        public CookieAuthenticationOptions TwoFactorRememberMeCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ClaimsIdentityOptions.DefaultTwoFactorRememberMeAuthenticationType,
            AuthenticationMode = AuthenticationMode.Passive
        };

        public CookieAuthenticationOptions TwoFactorUserIdCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ClaimsIdentityOptions.DefaultTwoFactorUserIdAuthenticationType,
            AuthenticationMode = AuthenticationMode.Passive
        };
    }
}
// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Security;
using Microsoft.AspNet.Security.Cookies;
using System;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Configuration for identity
    /// </summary>
    public class IdentityOptions
    {
        public ClaimsIdentityOptions ClaimsIdentity { get; set; } = new ClaimsIdentityOptions();

        public UserOptions User { get; set; } = new UserOptions();

        public PasswordOptions Password { get; set; } = new PasswordOptions();

        public LockoutOptions Lockout { get; set; } = new LockoutOptions();

        public SignInOptions SignIn { get; set; } = new SignInOptions();

        public TimeSpan SecurityStampValidationInterval { get; set; } = TimeSpan.FromMinutes(30);

        public string EmailConfirmationTokenProvider { get; set; } = Resources.DefaultTokenProvider;

        public string PasswordResetTokenProvider { get; set; } = Resources.DefaultTokenProvider;

        // Move these to CookieOptions sub type? (Make these constants/readonly?
        public static string ApplicationCookieAuthenticationType { get; set; } = typeof(IdentityOptions).Namespace + ".Application";
        public static string ExternalCookieAuthenticationType { get; set; } = typeof(IdentityOptions).Namespace + ".External";
        public static string TwoFactorUserIdCookieAuthenticationType { get; set; } = typeof(IdentityOptions).Namespace + ".TwoFactorUserId";
        public static string TwoFactorRememberMeCookieAuthenticationType { get; set; } = typeof(IdentityOptions).Namespace + ".TwoFactorRemeberMe";

        public CookieAuthenticationOptions ApplicationCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ClaimsIdentityOptions.DefaultAuthenticationType,
            //CookieName = ".AspNet.Identity." + ClaimsIdentityOptions.DefaultAuthenticationType,
            LoginPath = new PathString("/Account/Login"),
            Notifications = new CookieAuthenticationNotifications
            {
                OnValidateIdentity = SecurityStampValidator.ValidateIdentityAsync
            }
        };

        // Move to setups for named per cookie option
        public CookieAuthenticationOptions ExternalCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ExternalAuthenticationOptions.DefaultSignInAsAuthenticationType,
            AuthenticationMode = AuthenticationMode.Passive,
            CookieName = ClaimsIdentityOptions.DefaultExternalLoginAuthenticationType,
            ExpireTimeSpan = TimeSpan.FromMinutes(5),
        };

        public CookieAuthenticationOptions TwoFactorRememberMeCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ClaimsIdentityOptions.DefaultTwoFactorRememberMeAuthenticationType,
            AuthenticationMode = AuthenticationMode.Passive,
            CookieName = ClaimsIdentityOptions.DefaultTwoFactorRememberMeAuthenticationType
        };

        public CookieAuthenticationOptions TwoFactorUserIdCookie { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationType = ClaimsIdentityOptions.DefaultTwoFactorUserIdAuthenticationType,
            AuthenticationMode = AuthenticationMode.Passive,
            CookieName = ClaimsIdentityOptions.DefaultTwoFactorUserIdAuthenticationType,
            ExpireTimeSpan = TimeSpan.FromMinutes(5),
        };
    }
}
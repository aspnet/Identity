// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    /// Represents all the options you can use to configure the cookies middleware uesd by the identity system.
    /// </summary>
    public class IdentityCookieOptions
    {
        public IdentityCookieOptions()
        {
            // Configure all of the cookie middlewares
            ApplicationCookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = ApplicationCookieAuthenticationScheme,
                AutomaticAuthentication = true,
                LoginPath = new PathString("/Account/Login"),
                Notifications = new CookieAuthenticationNotifications
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                }
            };

            ExternalCookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = ExternalCookieAuthenticationScheme,
                CookieName = ExternalCookieAuthenticationScheme,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            };

            TwoFactorRememberMeCookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = TwoFactorRememberMeCookieAuthenticationScheme,
                CookieName = TwoFactorRememberMeCookieAuthenticationScheme
            };

            TwoFactorUserIdCookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = TwoFactorUserIdCookieAuthenticationScheme,
                CookieName = TwoFactorUserIdCookieAuthenticationScheme,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
                };

        }

        public CookieAuthenticationOptions ApplicationCookieOptions { get; set; }
        public CookieAuthenticationOptions ExternalCookieOptions { get; set; }
        public CookieAuthenticationOptions TwoFactorRememberMeCookieOptions { get; set; }
        public CookieAuthenticationOptions TwoFactorUserIdCookieOptions { get; set; }

        /// <summary>
        /// Gets or sets the scheme used to identify application authentication cookies.
        /// </summary>
        /// <value>The scheme used to identify application authentication cookies.</value>
        public string ApplicationCookieAuthenticationScheme { get; set; } = ApplicationCookieAuthenticationType;

        /// <summary>
        /// Gets or sets the scheme used to identify external authentication cookies.
        /// </summary>
        /// <value>The scheme used to identify external authentication cookies.</value>
        public string ExternalCookieAuthenticationScheme { get; set; } = typeof(IdentityCookieOptions).Namespace + ".External.AuthType";

        /// <summary>
        /// Gets or sets the scheme used to identify Two Factor authentication cookies for round tripping user identities.
        /// </summary>
        /// <value>The scheme used to identify user identity 2fa authentication cookies.</value>
        public string TwoFactorUserIdCookieAuthenticationScheme { get; set; } = typeof(IdentityCookieOptions).Namespace + ".TwoFactorUserId.AuthType";

        /// <summary>
        /// Gets or sets the scheme used to identify Two Factor authentication cookies for saving the Remember Me state.
        /// </summary>
        /// <value>The scheme used to identify remember me application authentication cookies.</value>        
        public string TwoFactorRememberMeCookieAuthenticationScheme { get; set; } = typeof(IdentityCookieOptions).Namespace + ".TwoFactorRemeberMe.AuthType";

        /// <summary>
        /// Gets or sets the authentication type used when constructing an <see cref="ClaimsIdentity"/> from an application cookie.
        /// </summary>
        /// <value>The authentication type used when constructing an <see cref="ClaimsIdentity"/> from an application cookie.</value>
        public static string ApplicationCookieAuthenticationType { get; set; } = typeof(IdentityCookieOptions).Namespace + ".Application.AuthType";
    }
}
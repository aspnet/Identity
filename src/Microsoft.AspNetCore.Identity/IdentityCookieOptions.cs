// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Represents all the options you can use to configure the cookies middleware uesd by the identity system.
    /// </summary>
    public class IdentityCookieOptions
    {
        private static readonly string CookiePrefix = "Identity";
        internal static readonly string DefaultApplicationScheme = CookiePrefix + ".Application";
        internal static readonly string DefaultExternalScheme = CookiePrefix + ".External";
        internal static readonly string DefaultTwoFactorRememberMeScheme = CookiePrefix + ".TwoFactorRememberMe";
        internal static readonly string DefaultTwoFactorUserIdScheme = CookiePrefix + ".TwoFactorUserId";

        /// <summary>
        /// The options for the application cookie.
        /// </summary>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", error: true)]
        public CookieAuthenticationOptions ApplicationCookie { get; set; }

        /// <summary>
        /// The options for the external cookie.
        /// </summary>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", error: true)]
        public CookieAuthenticationOptions ExternalCookie { get; set; }

        /// <summary>
        /// The options for the two factor remember me cookie.
        /// </summary>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", error: true)]
        public CookieAuthenticationOptions TwoFactorRememberMeCookie { get; set; }

        /// <summary>
        /// The options for the two factor user id cookie.
        /// </summary>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", error: true)]
        public CookieAuthenticationOptions TwoFactorUserIdCookie { get; set; }

        /// <summary>
        /// Gets the scheme used to identify application authentication cookies.
        /// </summary>
        /// <value>The scheme used to identify application authentication cookies.</value>
        public string ApplicationCookieAuthenticationScheme { get; set; } = DefaultApplicationScheme;

        /// <summary>
        /// Gets the scheme used to identify external authentication cookies.
        /// </summary>
        /// <value>The scheme used to identify external authentication cookies.</value>
        public string ExternalCookieAuthenticationScheme { get; set; } = DefaultExternalScheme;

        /// <summary>
        /// Gets the scheme used to identify Two Factor authentication cookies for round tripping user identities.
        /// </summary>
        /// <value>The scheme used to identify user identity 2fa authentication cookies.</value>
        public string TwoFactorUserIdCookieAuthenticationScheme { get; set; } = DefaultTwoFactorUserIdScheme;

        /// <summary>
        /// Gets the scheme used to identify Two Factor authentication cookies for saving the Remember Me state.
        /// </summary>
        /// <value>The scheme used to identify remember me application authentication cookies.</value>        
        public string TwoFactorRememberMeCookieAuthenticationScheme { get; set; } = DefaultTwoFactorRememberMeScheme;
    }
}

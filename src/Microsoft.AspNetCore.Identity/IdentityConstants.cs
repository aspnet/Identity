// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Represents all the options you can use to configure the cookies middleware uesd by the identity system.
    /// </summary>
    public class IdentityConstants
    {
        private const string CookiePrefix = "Identity";
        /// <summary>
        /// The scheme used to identify application authentication cookies.
        /// </summary>
        public const string ApplicationScheme = CookiePrefix + ".Application";

        /// <summary>
        /// The scheme used to identify external authentication cookies.
        /// </summary>
        public const string ExternalScheme = CookiePrefix + ".External";

        /// <summary>
        /// The scheme used to identify Two Factor authentication cookies for saving the Remember Me state.
        /// </summary>
        public const string TwoFactorRememberMeScheme = CookiePrefix + ".TwoFactorRememberMe";

        /// <summary>
        /// The scheme used to identify Two Factor authentication cookies for round tripping user identities.
        /// </summary>
        public const string TwoFactorUserIdScheme = CookiePrefix + ".TwoFactorUserId";
    }
}

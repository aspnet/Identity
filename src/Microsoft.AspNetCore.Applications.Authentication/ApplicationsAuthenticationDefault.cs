using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Applications.Authentication
{
    /// <summary>
    /// Constants for different application authentication values.
    /// </summary>
    public static class ApplicationsAuthenticationDefaults
    {
        /// <summary>
        /// The cookie authentication scheme used on an authenticated cookie.
        /// </summary>
        public const string CookieAuthenticationScheme = "Microsoft.AspNetCore.Applications.Authentication";
        
        /// <summary>
        /// The <see cref="AuthorizationPolicy"/> name used for login a user in.
        /// </summary>
        public const string LoginPolicyName = "Microsoft.AspNetCore.Applications.Authentication.Login";

        /// <summary>
        /// The <see cref="AuthorizationPolicy"/> name used for establishing a session for a user and an application.
        /// </summary>
        public const string SessionPolicyName = "Microsoft.AspNetCore.Applications.Authentication.Session";

        /// <summary>
        /// The <see cref="ManagementPolicyName"/> name used for managing applications on the identity provider.
        /// </summary>
        public const string ManagementPolicyName = "Microsoft.AspNetCore.Applications.Authentication.Management";

        /// <summary>
        /// The name used for the cookie on authentication.
        /// </summary>
        public const string CookieAuthenticationName = "Microsoft.AspNetCore.Applications.Authentication.Cookie";
    }
}

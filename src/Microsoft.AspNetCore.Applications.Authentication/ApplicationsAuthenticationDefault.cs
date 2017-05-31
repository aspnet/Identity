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
        public const string LoginPolicyName = "Microsoft.AspNetCore.Applications.Authentication.Login";
        public const string SessionPolicyName = "Microsoft.AspNetCore.Applications.Authentication.Session";
        public const string ManagementPolicyName = "Microsoft.AspNetCore.Applications.Authentication.Management";
        public const string CookieAuthenticationName = "Microsoft.AspNetCore.Applications.Authentication.Cookie";
    }
}

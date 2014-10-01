// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.Framework.ConfigurationModel;

namespace Microsoft.AspNet.Builder
{
    /// <summary>
    /// Startup extensions
    /// </summary>
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseIdentity(this IApplicationBuilder app, Action<IdentityOptions> configure = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (configure != null)
            {
                app.UseServices.SetupOptions(configure);
            }

            app.UseCookieAuthentication(IdentityOptions.ExternalCookieAuthenticationType);
            app.UseCookieAuthentication(IdentityOptions.ApplicationCookieAuthenticationType);
            app.UseCookieAuthentication(IdentityOptions.TwoFactorRememberMeCookieAuthenticationType);
            app.UseCookieAuthentication(IdentityOptions.TwoFactorUserIdCookieAuthenticationType);
            app.UseCookieAuthentication(IdentityOptions.ApplicationCookieAuthenticationType);
            return app;
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Builder
{
    /// <summary>
    /// Identity extensions for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Enables ASP.NET identity for the current application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance this method extends.</returns>
        public static IApplicationBuilder UseIdentity(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var marker = app.ApplicationServices.GetService<IdentityMarkerService>();
            if (marker == null)
            {
                throw new InvalidOperationException(Resources.MustCallAddIdentity);
            }

            var options = app.ApplicationServices.GetRequiredService<IOptions<IdentityOptions>>().Value;
            app.UseCookieAuthentication(options.Cookies.ExternalCookieOptions);
            app.UseCookieAuthentication(options.Cookies.TwoFactorRememberMeCookieOptions);
            app.UseCookieAuthentication(options.Cookies.TwoFactorUserIdCookieOptions);
            app.UseCookieAuthentication(options.Cookies.ApplicationCookieOptions);
            return app;
        }
    }
}
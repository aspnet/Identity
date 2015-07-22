// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

using Microsoft.AspNet.Identity;

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
            return app.UseIdentity(new IdentityCookieOptions());
        }

        /// <summary>
        /// Enables ASP.NET identity for the current application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance this method extends.</returns>
        public static IApplicationBuilder UseIdentity(this IApplicationBuilder app, IdentityCookieOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            app.UseCookieAuthentication(options.ExternalCookieOptions);
            app.UseCookieAuthentication(options.TwoFactorRememberMeCookieOptions);
            app.UseCookieAuthentication(options.TwoFactorUserIdCookieOptions);
            app.UseCookieAuthentication(options.ApplicationCookieOptions);
            return app;
        }
    }
}
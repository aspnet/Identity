// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.SignIn
{
    /// <summary>
    /// Helper functions for configuring identity services.
    /// </summary>
    public static class IdentitySignInCookieBuilderExtensions
    {
        /// <summary>
        /// Adds cookie authentication.
        /// </summary>
        /// <param name="builder">The current <see cref="IdentityBuilder"/> instance.</param>
        /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
        public static IdentityBuilder AddDefaultCookies(this IdentityBuilder builder)
        {
            // REVIEW: should the default config be moved into the helper methods instead?
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddApplicationCookie(o => { })
            .AddExternalCookie(o => { })
            .AddTwoFactorRememberMeCookie(o => { })
            .AddTwoFactorUserIdCookie(o => { });

            return builder;
        }

        /// <summary>
        /// Adds the identity application cookie.
        /// </summary>
        /// <param name="builder">The current <see cref="IdentityBuilder"/> instance.</param>
        /// <param name="configure">An action to configure the <see cref="CookieAuthenticationOptions"/>.</param>
        /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
        public static AuthenticationBuilder AddApplicationCookie(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> configure)
            => builder.AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = new PathString("/Account/Login");
                o.Events = new CookieAuthenticationEvents
                {
                    //OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };
                configure?.Invoke(o);
            });

        /// <summary>
        /// Adds the identity cookie used for external logins.
        /// </summary>
        /// <param name="builder">The current <see cref="IdentityBuilder"/> instance.</param>
        /// <param name="configure">An action to configure the <see cref="CookieAuthenticationOptions"/>.</param>
        /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
        public static AuthenticationBuilder AddExternalCookie(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> configure)
            => builder.AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                configure?.Invoke(o);
            });

        /// <summary>
        /// Adds the identity cookie used for two factor remember me.
        /// </summary>
        /// <param name="builder">The current <see cref="IdentityBuilder"/> instance.</param>
        /// <param name="configure">An action to configure the <see cref="CookieAuthenticationOptions"/>.</param>
        /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
        public static AuthenticationBuilder AddTwoFactorRememberMeCookie(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> configure)
            => builder.AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                configure?.Invoke(o);
            });

        /// <summary>
        /// Adds the identity cookie used for two factor logins.
        /// </summary>
        /// <param name="builder">The current <see cref="IdentityBuilder"/> instance.</param>
        /// <param name="configure">An action to configure the <see cref="CookieAuthenticationOptions"/>.</param>
        /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
        public static AuthenticationBuilder AddTwoFactorUserIdCookie(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> configure)
            => builder.AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                configure?.Invoke(o);
            });
    }
}

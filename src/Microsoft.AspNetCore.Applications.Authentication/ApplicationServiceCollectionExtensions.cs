// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Applications.Authentication;
using Microsoft.AspNetCore.Applications.Authentication.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.AspNetCore.Identity.Service.Session;
using Microsoft.AspNetCore.Identity.Service.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Identity
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IIdentityClientApplicationsBuilder AddApplications<TApplication>(this IdentityBuilder builder)
            where TApplication : class
        {
            var services = builder.Services;

            services.AddOptions();
            services.AddWebEncoders();
            services.AddDataProtection();
            services.AddAuthentication();

            builder.AddApplicationsCore<TApplication>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<AuthorizationOptions>, IdentityClientApplicationsAuthorizationOptionsSetup>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ApplicationTokenOptions>, TokenOptionsSetup>());

            services.TryAdd(CreateServices(builder.UserType, typeof(TApplication)));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITokenClaimsProvider, PairwiseSubClaimProvider>());

            services.AddAuthentication().AddCookie(ApplicationsAuthenticationDefaults.CookieAuthenticationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Path = "/tfp/Identity/signinsignup";
                options.AccessDeniedPath = "/tfp/Identity/signinsignup/Account/AccessDenied";
                options.Cookie.Name = ApplicationsAuthenticationDefaults.CookieAuthenticationName;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/tfp/Identity/signinsignup/Account/Login";
                options.AccessDeniedPath = "/tfp/Identity/signinsignup/Account/AccessDenied";
                options.Cookie.Path = "/tfp/Identity/signinsignup";
                options.Events = new CookieAuthenticationEvents()
                {
                    OnSigningOut = async ctx =>
                    {
                        await ctx.HttpContext.SignOutAsync(ApplicationsAuthenticationDefaults.CookieAuthenticationScheme);
                    }
                };
            });

            services.ConfigureExternalCookie(options => options.Cookie.Path = $"/tfp/Identity/signinsignup");
            services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorRememberMeScheme, options => options.Cookie.Path = $"/tfp/Identity");
            services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorUserIdScheme, options => options.Cookie.Path = $"/tfp/Identity");

            return new IdentityClientApplicationsBuilder<TApplication>(builder);
        }

        private static IEnumerable<ServiceDescriptor> CreateServices(Type userType, Type applicationType)
        {
            yield return ServiceDescriptor.Transient<ILoginContextProvider, LoginContextProvider>();

            var loginContextFactoryType = typeof(LoginContextFactory<,>).MakeGenericType(userType, applicationType);
            yield return ServiceDescriptor.Transient(typeof(ILoginFactory), loginContextFactoryType);
            yield return ServiceDescriptor.Transient(typeof(IRedirectUriResolver), typeof(ClientApplicationValidator<>).MakeGenericType(applicationType));
            yield return ServiceDescriptor.Singleton<FormPostResponseGenerator, FormPostResponseGenerator>();
            yield return ServiceDescriptor.Singleton<FragmentResponseGenerator, FragmentResponseGenerator>();
            yield return ServiceDescriptor.Singleton<QueryResponseGenerator, QueryResponseGenerator>();

            yield return ServiceDescriptor.Transient(typeof(IClientIdValidator), typeof(ClientApplicationValidator<>).MakeGenericType(applicationType));
            yield return ServiceDescriptor.Transient(typeof(IScopeResolver), typeof(ClientApplicationValidator<>).MakeGenericType(applicationType));
            yield return ServiceDescriptor.Singleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static IdentityBuilder AddApplications<TApplication>(
            this IdentityBuilder builder,
            Action<ApplicationOptions> configure) where TApplication : class
        {
            builder.AddApplications<TApplication>();
            builder.Services.Configure(configure);
            return builder;
        }

        public static IdentityBuilder AddApplications(
            this IdentityBuilder builder,
            Action<ApplicationOptions> configure)
        {
            builder.Services.Configure(configure);
            return builder;
        }

        public static IdentityBuilder AddApplications(this IdentityBuilder builder)
        {
            return builder;
        }
    }
}

﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.AspNetCore.Identity.Service.Configuration;
using Microsoft.AspNetCore.Identity.Service.Core;
using Microsoft.AspNetCore.Identity.Service.Metadata;
using Microsoft.AspNetCore.Identity.Service.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceServiceCollectionExtensions
    {
        public static IIdentityServiceBuilder AddApplications<TUser, TApplication>(
            this IdentityBuilder builder,
            Action<IdentityServiceOptions> configure)
            where TUser : class
            where TApplication : class
        {
            if (builder == null)
            {
                throw new NullReferenceException(nameof(builder));
            }

            if (configure == null)
            {
                throw new NullReferenceException(nameof(configure));
            }

            var services = builder.Services;

            services.AddOptions();
            services.AddWebEncoders();
            services.AddDataProtection();
            services.AddAuthentication();

            services.TryAdd(CreateServices<TUser, TApplication>());

            // Configuration
            services.AddTransient<IConfigureOptions<IdentityServiceOptions>, IdentityServiceOptionsDefaultSetup>();
            services.AddTransient<IConfigureOptions<IdentityServiceOptions>, IdentityServiceOptionsSetup>();

            services.AddCookieAuthentication(IdentityServiceOptions.CookieAuthenticationScheme, options =>
            {
                options.CookieHttpOnly = true;
                options.CookieSecure = CookieSecurePolicy.Always;
                options.CookiePath = "/tfp/IdentityService/signinsignup";
                options.CookieName = IdentityServiceOptions.AuthenticationCookieName;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/tfp/IdentityService/Account/Login";
                options.AccessDeniedPath = $"/tfp/IdentityService/Account/Denied";
                options.CookiePath = $"/tfp/IdentityService";
            });
            services.ConfigureApplicationCookie(options => options.CookiePath = $"/tfp/IdentityService");
            services.Configure<CookieAuthenticationOptions>(IdentityCookieOptions.TwoFactorRememberMeScheme, options => options.CookiePath = $"/tfp/IdentityService");
            services.Configure<CookieAuthenticationOptions>(IdentityCookieOptions.TwoFactorUserIdScheme, options => options.CookiePath = $"/tfp/IdentityService");

            services.AddTransient<IConfigureOptions<AuthorizationOptions>, IdentityServiceAuthorizationOptionsSetup>();

            // Other stuff
            services.AddSingleton<IAuthorizationResponseFactory, DefaultAuthorizationResponseFactory>();
            services.AddSingleton<IAuthorizationResponseParameterProvider, DefaultAuthorizationResponseParameterProvider>();
            services.AddSingleton<ITokenResponseFactory, DefaultTokenResponseFactory>();
            services.AddSingleton<ITokenResponseParameterProvider, DefaultTokenResponseParameterProvider>();
            services.AddSingleton<ITokenClaimsManager, DefaultTokenClaimsManager>();
            services.AddSingleton<ITokenClaimsProvider, PairwiseSubClaimProvider>();
            services.AddSingleton<ITokenClaimsProvider, DefaultTokenClaimsProvider>();
            services.AddSingleton<ITokenClaimsProvider, GrantedTokensTokenClaimsProvider>();
            services.AddSingleton<ITokenClaimsProvider, NonceTokenClaimsProvider>();
            services.AddSingleton<ITokenClaimsProvider, ScopesTokenClaimsProvider>();
            services.AddSingleton<ITokenClaimsProvider, TimestampsTokenClaimsProvider>();
            services.AddSingleton<ITokenClaimsProvider, TokenHashTokenClaimsProvider>();
            services.AddSingleton<ProtocolErrorProvider>();

            services.AddSingleton<IPasswordHasher<TApplication>, PasswordHasher<TApplication>>();
            services.AddScoped<ISigningCredentialsPolicyProvider, DefaultSigningCredentialsPolicyProvider>();
            services.AddScoped<ISigningCredentialsSource, DefaultSigningCredentialsSource>();

            // Session
            services.AddTransient<SessionManager, SessionManager<TUser, TApplication>>();
            services.AddTransient<SessionManager<TUser, TApplication>>();
            services.AddTransient<IRedirectUriResolver, ClientApplicationValidator<TApplication>>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure(configure);

            return new IdentityServiceBuilder<TApplication>(builder);
        }

        private static IEnumerable<ServiceDescriptor> CreateServices<TUser, TApplication>()
            where TUser : class
            where TApplication : class
        {
            yield return ServiceDescriptor.Scoped<ApplicationManager<TApplication>, ApplicationManager<TApplication>>();

            // Protocol services
            yield return ServiceDescriptor.Transient<ITokenManager, TokenManager>();
            yield return ServiceDescriptor.Transient<IAuthorizationCodeIssuer, AuthorizationCodeIssuer>();
            yield return ServiceDescriptor.Transient<IAccessTokenIssuer, JwtAccessTokenIssuer>();
            yield return ServiceDescriptor.Transient<IIdTokenIssuer, JwtIdTokenIssuer>();
            yield return ServiceDescriptor.Transient<IRefreshTokenIssuer, RefreshTokenIssuer>();
            yield return ServiceDescriptor.Transient<IKeySetMetadataProvider, DefaultKeySetMetadataProvider>();

            // Infrastructure services
            yield return ServiceDescriptor.Singleton<ITimeStampManager, TimeStampManager>();
            yield return ServiceDescriptor.Transient<ITokenHasher, TokenHasher>();
            yield return ServiceDescriptor.Singleton<FormPostResponseGenerator, FormPostResponseGenerator>();
            yield return ServiceDescriptor.Singleton<FragmentResponseGenerator, FragmentResponseGenerator>();
            yield return ServiceDescriptor.Singleton<QueryResponseGenerator, QueryResponseGenerator>();
            yield return ServiceDescriptor.Transient<ISecureDataFormat<AuthorizationCode>, SecureDataFormat<AuthorizationCode>>();
            yield return ServiceDescriptor.Transient<ISecureDataFormat<RefreshToken>, SecureDataFormat<RefreshToken>>();
            yield return ServiceDescriptor.Singleton(sp => sp.GetDataProtectionProvider().CreateProtector("IdentityProvider"));
            yield return ServiceDescriptor.Transient<JwtSecurityTokenHandler, JwtSecurityTokenHandler>();
            yield return ServiceDescriptor.Transient<IDataSerializer<AuthorizationCode>, TokenDataSerializer<AuthorizationCode>>();
            yield return ServiceDescriptor.Transient<IDataSerializer<RefreshToken>, TokenDataSerializer<RefreshToken>>();
            yield return ServiceDescriptor.Transient<IApplicationClaimsPrincipalFactory<TApplication>, ApplicationClaimsPrincipalFactory<TApplication>>();

            // Validation
            yield return ServiceDescriptor.Transient<IAuthorizationRequestFactory, AuthorizationRequestFactory>();
            yield return ServiceDescriptor.Transient<ITokenRequestFactory, TokenRequestFactory>();
            yield return ServiceDescriptor.Transient<ILogoutRequestFactory, LogoutRequestFactory>();
            yield return ServiceDescriptor.Transient<IClientIdValidator, ClientApplicationValidator<TApplication>>();
            yield return ServiceDescriptor.Transient<IScopeResolver, ClientApplicationValidator<TApplication>>();

            // Metadata
            yield return ServiceDescriptor.Singleton<IConfigurationManager, DefaultConfigurationManager>();
            yield return ServiceDescriptor.Singleton<IConfigurationMetadataProvider, DefaultConfigurationMetadataProvider>();
        }
    }
}

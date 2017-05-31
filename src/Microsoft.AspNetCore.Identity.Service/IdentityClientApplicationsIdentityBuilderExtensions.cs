// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityClientApplicationsIdentityBuilderExtensions
    {
        public static IIdentityClientApplicationsBuilder AddApplicationsCore<TApplication>(
            this IdentityBuilder builder,
            Action<ApplicationTokenOptions> configure)
            where TApplication : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var identityApplications = builder.AddApplicationsCore<TApplication>();
            builder.Services.Configure(configure);

            return identityApplications;
        }

        public static IIdentityClientApplicationsBuilder AddApplicationsCore<TApplication>(
            this IdentityBuilder builder)
            where TApplication : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var services = builder.Services;

            services.AddApplicationTokens();
            services.TryAdd(CreateServices<TApplication>());

            return new IdentityClientApplicationsBuilder<TApplication>(builder);
        }

        private static IEnumerable<ServiceDescriptor> CreateServices<TApplication>()
            where TApplication : class
        {
            yield return ServiceDescriptor.Transient<IApplicationClaimsPrincipalFactory<TApplication>, ApplicationClaimsPrincipalFactory<TApplication>>();
            yield return ServiceDescriptor.Singleton<IPasswordHasher<TApplication>, PasswordHasher<TApplication>>();
            yield return ServiceDescriptor.Singleton<IApplicationValidator<TApplication>, ApplicationValidator<TApplication>>();
            yield return ServiceDescriptor.Singleton(new ApplicationErrorDescriber());
            yield return ServiceDescriptor.Scoped<ApplicationManager<TApplication>, ApplicationManager<TApplication>>();
        }
    }
}

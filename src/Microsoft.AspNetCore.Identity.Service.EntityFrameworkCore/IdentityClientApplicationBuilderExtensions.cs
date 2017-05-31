// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.Service.EntityFrameworkCore
{
    public static class IdentityClientApplicationBuilderExtensions
    {
        public static IIdentityClientApplicationsBuilder AddApplicationsCore(
            this IdentityBuilder builder,
            Action<ApplicationTokenOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            return builder.AddApplicationsCore<IdentityClientApplication>(configure);
        }

        public static IIdentityClientApplicationsBuilder AddApplicationsCore(
            this IdentityBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddApplicationsCore<IdentityClientApplication>();
        }

        public static IIdentityClientApplicationsBuilder AddEntityFrameworkStores<TContext>(this IIdentityClientApplicationsBuilder builder)
            where TContext : DbContext
        {
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.RoleType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<TContext>();

            var services = builder.Services;
            var applicationType = FindGenericBaseType(builder.ApplicationType, typeof(IdentityClientApplication<,,,>));

            services.AddTransient(
                typeof(IApplicationStore<>).MakeGenericType(builder.ApplicationType),
                typeof(ApplicationStore<,,,,,>).MakeGenericType(
                    builder.ApplicationType,
                    applicationType.GenericTypeArguments[1],
                    applicationType.GenericTypeArguments[2],
                    applicationType.GenericTypeArguments[3],
                    typeof(TContext),
                    applicationType.GenericTypeArguments[0]));

            return builder;
        }

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType.GetTypeInfo();
            while (type.BaseType != null)
            {
                type = type.BaseType.GetTypeInfo();
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return type;
                }
            }
            return null;
        }
    }
}

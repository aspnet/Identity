﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.Service.EntityFrameworkCore
{
    public static class IdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder AddApplications(
            this IdentityBuilder builder,
            Action<IdentityServiceOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            return builder.AddApplications<IdentityServiceApplication>(configure);
        }

        public static IIdentityServiceBuilder AddApplications(
            this IdentityBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddApplications<IdentityServiceApplication>();
        }

        public static IIdentityServiceBuilder AddEntityFrameworkStores<TContext>(this IIdentityServiceBuilder builder)
            where TContext : DbContext
        {
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.RoleType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<TContext>();

            var services = builder.Services;
            var applicationType = FindGenericBaseType(builder.ApplicationType, typeof(IdentityServiceApplication<,,,,>));
            var userType = FindGenericBaseType(builder.UserType, typeof(IdentityUser<>));

            services.AddTransient(
                typeof(IApplicationStore<>).MakeGenericType(builder.ApplicationType),
                typeof(ApplicationStore<,,,,,,>).MakeGenericType(
                    builder.ApplicationType,
                    applicationType.GenericTypeArguments[2],
                    applicationType.GenericTypeArguments[3],
                    applicationType.GenericTypeArguments[4],
                    typeof(TContext),
                    applicationType.GenericTypeArguments[0],
                    userType.GenericTypeArguments[0]));

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

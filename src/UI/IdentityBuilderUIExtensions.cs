﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Default UI extensions to <see cref="IdentityBuilder"/>.
    /// </summary>
    public static class IdentityBuilderUIExtensions
    {
        /// <summary>
        /// Adds a default, self-contained UI for Identity to the application using
        /// Razor Pages in an area named Identity.
        /// </summary>
        /// <remarks>
        /// In order to use the default UI, the application must be using <see cref="Microsoft.AspNetCore.Mvc"/>,
        /// <see cref="Microsoft.AspNetCore.StaticFiles"/> and contain a <c>_LoginPartial</c> partial view that
        /// can be found by the application.
        /// </remarks>
        /// <param name="builder">The <see cref="IdentityBuilder"/>.</param>
        /// <returns>The <see cref="IdentityBuilder"/>.</returns>
        public static IdentityBuilder AddDefaultUI(this IdentityBuilder builder)
        {
            builder.AddSignInManager();
            AddRelatedParts(builder);

            builder.Services.ConfigureOptions(
                typeof(IdentityDefaultUIConfigureOptions<>)
                    .MakeGenericType(builder.UserType));
            builder.Services.TryAddTransient<IEmailSender, EmailSender>();

            // Hookup the UserFactory for IdentityUser based user types
            if (TryGetIdentityUserType(builder.UserType, out var primaryKeyType))
            {
                var userFactoryType = typeof(UserFactory<>).MakeGenericType(builder.UserType);
                var defaultUserFactoryType = typeof(IdentityUserFactory<,>).MakeGenericType(builder.UserType, primaryKeyType);
                builder.Services.TryAddSingleton(userFactoryType, defaultUserFactoryType);
            }

            return builder;
        }

        private static void AddRelatedParts(IdentityBuilder builder)
        {
            // For preview1, we don't have a good mechanism to plug in additional parts.
            // We need to provide API surface to allow libraries to plug in existing parts
            // that were optionally added.
            // Challenges here are:
            // * Discovery of the parts.
            // * Ordering of the parts.
            // * Loading of the assembly in memory.

            var mvcBuilder = builder.Services
                .AddMvc()
                .ConfigureApplicationPartManager(partManager =>
                {
                    var thisAssembly = typeof(IdentityBuilderUIExtensions).Assembly;
                    var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(thisAssembly, throwOnError: true);
                    var relatedParts = relatedAssemblies.SelectMany(CompiledRazorAssemblyApplicationPartFactory.GetDefaultApplicationParts);

                    foreach (var part in relatedParts)
                    {
                        partManager.ApplicationParts.Add(part);
                    }
                });
        }

        private static bool TryGetIdentityUserType(Type userType, out Type primaryKeyType)
        {
            primaryKeyType = null;

            var baseType = userType.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == typeof(IdentityUser<>))
                {
                    primaryKeyType = baseType.GetGenericArguments()[0];
                    return true;
                }
                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}

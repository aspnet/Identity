// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Helper functions for configuring identity services.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="SignInManager{TUser}"/> for the <seealso cref="IdentityBuilder.UserType"/>.
        /// </summary>
        /// <typeparam name="TSignInManager">The type of the sign in manager to add.</typeparam>
        /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
        public static IdentityBuilder AddSignInManager<TSignInManager>(this IdentityBuilder builder) where TSignInManager : class
        {
            var managerType = typeof(SignInManager<>).MakeGenericType(builder.UserType);
            var customType = typeof(TSignInManager);
            if (managerType == customType ||
                !managerType.GetTypeInfo().IsAssignableFrom(customType.GetTypeInfo()))
            {
                throw new InvalidOperationException(AspNetIdentityResources.FormatInvalidManagerType(customType.Name, "SignInManager", builder.UserType.Name));
            }
            builder.Services.AddScoped(typeof(TSignInManager), services => services.GetRequiredService(managerType));
            builder.Services.AddScoped(managerType, typeof(TSignInManager));
            return builder;
        }
    }
}

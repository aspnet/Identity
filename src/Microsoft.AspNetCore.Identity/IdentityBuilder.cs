// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Allows fine grained configuration of identity services.
    /// </summary>
    public class IdentityBuilder : IIdentityBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="IdentityBuilder"/>.
        /// </summary>
        /// <param name="user">The <see cref="Type"/> to use for the users.</param>
        /// <param name="role">The <see cref="Type"/> to use for the roles.</param>
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        public IdentityBuilder(Type user, Type role, IServiceCollection services)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            UserType = user;
            RoleType = role;
            Services = services;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public Type UserType { get; }

        /// <inheritdoc />
        public Type RoleType { get; }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity.EntityFramework6
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The default implementation of <see cref="IdentityRole{TKey}"/> which uses a string as the primary key.
    /// </summary>
    public class IdentityRole : IdentityRole<string>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to from a new GUID string value.
        /// </remarks>
        public IdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <remarks>
        /// The Id property is initialized to from a new GUID string value.
        /// </remarks>
        public IdentityRole(string roleName) : this()
        {
            Name = roleName;
        }
    }

    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the role.</typeparam>
    public class IdentityRole<TKey> : IdentityRole<TKey, IdentityUserRole<TKey>, IdentityRoleClaim<TKey>>
        where TKey : IEquatable<TKey>
    {
    }

    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the role.</typeparam>
    /// <typeparam name="TUserRole">The type used for user roles.</typeparam>
    /// <typeparam name="TRoleClaim">The type used for role claims.</typeparam>
    public class IdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole{TKey}"/>.
        /// </summary>
        public IdentityRole() { }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole{TKey}"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public IdentityRole(string roleName) : this()
        {
            Name = roleName;
        }

        /// <summary>
        /// Navigation property for the users in this role.
        /// </summary>
        public virtual ICollection<TUserRole> Users { get; } = new List<TUserRole>();

        /// <summary>
        /// Navigation property for claims in this role.
        /// </summary>
        public virtual ICollection<TRoleClaim> Claims { get; } = new List<TRoleClaim>();

        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        public virtual string NormalizedName { get; set; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Returns the name of the role.
        /// </summary>
        /// <returns>The name of the role.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
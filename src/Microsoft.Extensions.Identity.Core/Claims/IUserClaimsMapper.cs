// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.Claims
{
    /// <summary>
    /// Maps properties from the <typeparamref name="TUser"/> into <see cref="Claim"/>s in the <see cref="ClaimsIdentity"/>.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public interface IUserClaimsMapper<TUser>
        where TUser : class
    {
        /// <summary>
        /// Maps properties from the <paramref name="user"/> into the <paramref name="identity"/>.
        /// </summary>
        /// <param name="user">The user to map properties into claims from.</param>
        /// <param name="identity">The claims identity to add the claims to.</param>
        void Map(TUser user, ClaimsIdentity identity);
    }
}
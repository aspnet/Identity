// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore
{
    /// <summary>
    /// Represents a claim that is granted to all users within a role.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key of the role associated with this claim.</typeparam>
    public class IdentityRoleClaim<TKey> : Microsoft.Extensions.Identity.IdentityRoleClaim<TKey> where TKey : IEquatable<TKey>
    { }
}
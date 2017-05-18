// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore
{
    /// <summary>
    /// Represents a login and its associated provider for a user.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key of the user associated with this login.</typeparam>
    public class IdentityUserLogin<TKey> : Microsoft.Extensions.Identity.IdentityUserLogin<TKey> where TKey : IEquatable<TKey>
    { }
}
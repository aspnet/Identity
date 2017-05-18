// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key used for users.</typeparam>
    public class IdentityUserToken<TKey> : Microsoft.Extensions.Identity.IdentityUserToken<TKey> where TKey : IEquatable<TKey>
    { }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;

namespace Microsoft.AspNetCore.Identity
{
    public interface IQueryableClientStore<TClient> : IClientStore<TClient> where TClient : class
    {
        IQueryable<TClient> Clients { get; }
    }
}

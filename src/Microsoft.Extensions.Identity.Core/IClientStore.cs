// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public interface IClientStore<TClient> : IDisposable where TClient : class
    {
        Task<IdentityResult> CreateAsync(TClient client, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(TClient client, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TClient client, CancellationToken cancellationToken);
        Task<TClient> FindByIdAsync(string clientId, CancellationToken cancellationToken);
        Task<IEnumerable<TClient>> FindByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<TClient> FindByNameAsync(string name, CancellationToken cancellationToken);
        Task<string> GetIdAsync(TClient client, CancellationToken cancellationToken);
        Task<string> GetNameAsync(TClient client, CancellationToken cancellationToken);
        Task<string> GetUserIdAsync(TClient client, CancellationToken cancellationToken);
    }
}

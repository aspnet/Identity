// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public interface IClientScopeStore<TClient> : IClientStore<TClient> where TClient : class
    {
        Task<IEnumerable<string>> FindScopesAsync(TClient client, CancellationToken cancellationToken);
        Task<IdentityResult> AddScopeAsync(TClient client, string scope, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateScopeAsync(TClient client, string oldScope, string newScope, CancellationToken cancellationToken);
        Task<IdentityResult> RemoveScopeAsync(TClient client, string scope, CancellationToken cancellationToken);
    }
}

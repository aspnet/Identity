// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public interface IClientClaimStore<TClient> : IClientStore<TClient> where TClient : class
    {
        Task<IList<Claim>> GetClaimsAsync(TClient client, CancellationToken cancellationToken);
        Task AddClaimsAsync(TClient client, IEnumerable<Claim> claims, CancellationToken cancellationToken);
        Task ReplaceClaimAsync(TClient client, Claim claim, Claim newClaim, CancellationToken cancellationToken);
        Task RemoveClaimsAsync(TClient client, IEnumerable<Claim> claims, CancellationToken cancellationToken);
    }
}
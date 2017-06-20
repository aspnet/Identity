// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public interface IClientRedirectUriStore<TClient> : IClientStore<TClient> where TClient : class
    {
        Task<IEnumerable<string>> FindRegisteredUrisAsync(TClient client, CancellationToken cancellationToken);
        Task<IEnumerable<string>> FindRegisteredLogoutUrisAsync(TClient client, CancellationToken cancellationToken);
        Task<IdentityResult> RegisterRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);
        Task<IdentityResult> RegisterLogoutRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);
        Task<IdentityResult> UnregisterRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);
        Task<IdentityResult> UnregisterLogoutRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateRedirectUriAsync(TClient client, string oldRedirectUri, string newRedirectUri, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateLogoutRedirectUriAsync(TClient client, string oldRedirectUri, string newRedirectUri, CancellationToken cancellationToken);
    }
}

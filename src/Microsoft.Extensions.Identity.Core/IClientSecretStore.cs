// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public interface IClientSecretStore<TClient> : IClientStore<TClient> where TClient : class
    {
        Task SetClientSecretHashAsync(TClient client, string hash, CancellationToken cancellationToken);
        Task<string> GetClientSecretHashAsync(TClient client, CancellationToken cancellationToken);
        Task<bool> HasClientSecretAsync(TClient client, CancellationToken cancellationToken);
    }
}

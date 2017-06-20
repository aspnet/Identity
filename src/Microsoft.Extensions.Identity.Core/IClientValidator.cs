// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public interface IClientValidator<TClient> where TClient : class
    {
        Task<IdentityResult> ValidateAsync(ClientManager<TClient> manager, TClient Client);
    }
}

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    public interface IUserMessageProvider<TUser> where TUser : class
    {
        string Name { get; }

        Task SendAsync(UserManager<TUser> manager, TUser user, IdentityMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
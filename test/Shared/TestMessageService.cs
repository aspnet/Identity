// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.Test
{
    public class TestMessageService<TUser> : IUserMessageProvider<TUser> where TUser : class
    {
        public IdentityMessage Message { get; set; }

        public string Name { get; set; } = "Test";

        public Task SendAsync(UserManager<TUser> manager, TUser user, IdentityMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            Message = message;
            return Task.FromResult(0);
        }
    }

}
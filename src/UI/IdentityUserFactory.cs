// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity.UI
{
    internal class IdentityUserFactory<TUser, TKey> : UserFactory<TUser>
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        public override TUser CreateUser(string userName, string email)
            => new TUser() { Email = email, UserName = userName };
    }
}

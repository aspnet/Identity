// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Identity
{
    public class IdentityClient : IdentityClient<string> { }

    public class IdentityClient<TUserKey> :
        IdentityClient<TUserKey, IdentityClientScope, IdentityClientClaim, IdentityClientRedirectUri>
        where TUserKey : IEquatable<TUserKey>
    { }

    public class IdentityClient<TUserKey, TScope, TApplicationClaim, TRedirectUri>
        where TUserKey : IEquatable<TUserKey>
        where TScope : IdentityClientScope
        where TApplicationClaim : IdentityClientClaim
        where TRedirectUri : IdentityClientRedirectUri
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TUserKey UserId { get; set; }
        public string ClientSecretHash { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<TScope> Scopes { get; set; }
        public ICollection<TApplicationClaim> Claims { get; set; }
        public ICollection<TRedirectUri> RedirectUris { get; set; }
    }
}

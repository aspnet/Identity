// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class IdentityClientApplication : IdentityClientApplication<string>
    {
    }

    public class IdentityClientApplication<TApplicationKey> :
        IdentityClientApplication<
            TApplicationKey,
            IdentityClientApplicationScope<TApplicationKey>,
            IdentityClientApplicationClaim<TApplicationKey>,
            IdentityClientApplicationRedirectUri<TApplicationKey>>
        where TApplicationKey : IEquatable<TApplicationKey>
    {
    }

    public class IdentityClientApplication<TKey, TScope, TApplicationClaim, TRedirectUri>
        where TKey : IEquatable<TKey>
        where TScope : IdentityClientApplicationScope<TKey>
        where TApplicationClaim : IdentityClientApplicationClaim<TKey>
        where TRedirectUri : IdentityClientApplicationRedirectUri<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecretHash { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<TScope> Scopes { get; set; }
        public ICollection<TApplicationClaim> Claims { get; set; }
        public ICollection<TRedirectUri> RedirectUris { get; set; }
    }
}

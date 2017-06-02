// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity
{
    public class IdentityClientRedirectUri
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public bool IsLogout { get; set; }
        public string Value { get; set; }
    }
}

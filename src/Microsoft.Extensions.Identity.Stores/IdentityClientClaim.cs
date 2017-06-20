﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity
{
    public class IdentityClientClaim
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public Claim ToClaim() => new Claim(ClaimType, ClaimValue);
    }
}

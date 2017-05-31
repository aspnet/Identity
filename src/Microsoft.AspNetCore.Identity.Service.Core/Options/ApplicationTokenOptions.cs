// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class ApplicationTokenOptions
    {
        public string Issuer { get; set; }

        public IList<SigningCredentials> SigningKeys { get; set; } = new List<SigningCredentials>();

        public TokenClaimsOptions AuthorizationCodeOptions { get; set; } = new TokenClaimsOptions();

        public TokenClaimsOptions AccessTokenOptions { get; set; } = new TokenClaimsOptions();

        public TokenClaimsOptions RefreshTokenOptions { get; set; } = new TokenClaimsOptions();

        public TokenClaimsOptions IdTokenOptions { get; set; } = new TokenClaimsOptions();

        public JsonSerializerSettings SerializationSettings { get; set; }
    }
}

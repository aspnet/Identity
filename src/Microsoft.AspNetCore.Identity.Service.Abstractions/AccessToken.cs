// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class AccessToken : Token
    {
        public AccessToken(IEnumerable<Claim> claims)
            : base(ValidateClaims(claims))
        {
        }

        private static IEnumerable<Claim> ValidateClaims(IEnumerable<Claim> claims)
        {
            EnsureUniqueClaim(TokenClaimTypes.Issuer, claims);
            EnsureUniqueClaim(TokenClaimTypes.Subject, claims);
            EnsureUniqueClaim(TokenClaimTypes.Audience, claims);
            EnsureUniqueClaim(TokenClaimTypes.Scope, claims);
            EnsureUniqueClaim(TokenClaimTypes.AuthorizedParty, claims);
            return claims;
        }

        public override string Kind => TokenTypes.AccessToken;
        public string Issuer => GetClaimValue(TokenClaimTypes.Issuer);
        public string Subject => GetClaimValue(TokenClaimTypes.Subject);
        public string Audience => GetClaimValue(TokenClaimTypes.Audience);
        public string AuthorizedParty => GetClaimValue(TokenClaimTypes.AuthorizedParty);
        public IEnumerable<string> Scopes => GetClaimValuesOrEmpty(TokenClaimTypes.Scope);
    }
}

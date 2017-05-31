// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class RefreshToken : Token
    {
        public RefreshToken(IEnumerable<Claim> claims)
            : base(ValidateClaims(claims))
        {
        }

        private static IEnumerable<Claim> ValidateClaims(IEnumerable<Claim> claims)
        {
            EnsureUniqueClaim(TokenClaimTypes.UserId, claims);
            EnsureUniqueClaim(TokenClaimTypes.ClientId, claims);
            EnsureUniqueClaim(TokenClaimTypes.Scope, claims);
            EnsureRequiredClaim(TokenClaimTypes.GrantedToken, claims);

            return claims;
        }

        public override string Kind => TokenTypes.RefreshToken;
        public string UserId => GetClaimValue(TokenClaimTypes.UserId);
        public string ClientId => GetClaimValue(TokenClaimTypes.ClientId);
        public string Resource => GetClaimValue(TokenClaimTypes.Resource);
        public string Issuer => GetClaimValue(TokenClaimTypes.Issuer);
        public IEnumerable<string> GrantedTokens => GetClaimValuesOrEmpty(TokenClaimTypes.GrantedToken);
        public IEnumerable<string> Scopes => GetClaimValuesOrEmpty(TokenClaimTypes.Scope);
    }
}

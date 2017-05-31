// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class IdToken : Token
    {
        public IdToken(IEnumerable<Claim> claims)
            : base(ValidateClaims(claims))
        {
        }

        private static IEnumerable<Claim> ValidateClaims(IEnumerable<Claim> claims)
        {
            EnsureUniqueClaim(TokenClaimTypes.Issuer, claims);
            EnsureUniqueClaim(TokenClaimTypes.Subject, claims);
            EnsureUniqueClaim(TokenClaimTypes.Audience, claims);
            EnsureUniqueClaim(TokenClaimTypes.Nonce, claims, required: false);
            EnsureUniqueClaim(TokenClaimTypes.CodeHash, claims, required: false);
            EnsureUniqueClaim(TokenClaimTypes.AccessTokenHash, claims, required: false);
            return claims;
        }

        public override string Kind => TokenTypes.IdToken;
        public string Issuer => GetClaimValue(TokenClaimTypes.Issuer);
        public string Subject => GetClaimValue(TokenClaimTypes.Subject);
        public string Audience => GetClaimValue(TokenClaimTypes.Audience);
        public string Nonce => GetClaimValue(TokenClaimTypes.Nonce);
        public string CodeHash => GetClaimValue(TokenClaimTypes.CodeHash);
        public string AccessTokenHash => GetClaimValue(TokenClaimTypes.AccessTokenHash);
    }
}

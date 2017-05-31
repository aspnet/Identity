// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class AuthorizationCode : Token
    {
        public AuthorizationCode(IEnumerable<Claim> claims)
            : base(ValidateClaims(claims))
        {
        }

        private static IEnumerable<Claim> ValidateClaims(IEnumerable<Claim> claims)
        {
            EnsureUniqueClaim(TokenClaimTypes.UserId, claims);
            EnsureUniqueClaim(TokenClaimTypes.ClientId, claims);
            EnsureUniqueClaim(TokenClaimTypes.RedirectUri, claims);
            EnsureUniqueClaim(TokenClaimTypes.Scope, claims);
            EnsureRequiredClaim(TokenClaimTypes.GrantedToken, claims);

            return claims;
        }

        public override string Kind => TokenTypes.AuthorizationCode;
        public string UserId => GetClaimValue(TokenClaimTypes.UserId);
        public string ClientId => GetClaimValue(TokenClaimTypes.ClientId);
        public string Resource => GetClaimValue(TokenClaimTypes.Resource);
        public string RedirectUri => GetClaimValue(TokenClaimTypes.RedirectUri);
        public string CodeChallenge => GetClaimValue(TokenClaimTypes.CodeChallenge);
        public string CodeChallengeMethod => GetClaimValue(TokenClaimTypes.CodeChallengeMethod);
        public IEnumerable<string> Scopes => GetClaimValuesOrEmpty(TokenClaimTypes.Scope);
        public IEnumerable<string> GrantedTokens => GetClaimValuesOrEmpty(TokenClaimTypes.GrantedToken);
        public string Nonce => GetClaimValue(TokenClaimTypes.Nonce);
    }
}

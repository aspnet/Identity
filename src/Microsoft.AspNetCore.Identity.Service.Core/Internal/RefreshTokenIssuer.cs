// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.AspNetCore.Identity.Service.Issuers;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class RefreshTokenIssuer : IRefreshTokenIssuer
    {
        private static readonly string[] ClaimsToFilter = new string[]
        {
            TokenClaimTypes.ObjectId,
            TokenClaimTypes.Issuer,
            TokenClaimTypes.Audience,
            TokenClaimTypes.IssuedAt,
            TokenClaimTypes.Expires,
            TokenClaimTypes.NotBefore,
        };

        private static readonly string[] ClaimsToExclude = new string[]
        {
            TokenClaimTypes.JwtId,
            TokenClaimTypes.Issuer,
            TokenClaimTypes.Subject,
            TokenClaimTypes.Audience,
            TokenClaimTypes.Scope,
            TokenClaimTypes.IssuedAt,
            TokenClaimTypes.Expires,
            TokenClaimTypes.NotBefore,
        };

        private readonly ITokenProtector<RefreshToken> _dataFormat;
        private readonly ITokenClaimsManager _claimsManager;

        public RefreshTokenIssuer(
            ITokenClaimsManager claimsManager,
            ITokenProtector<RefreshToken> dataFormat)
        {
            _claimsManager = claimsManager;
            _dataFormat = dataFormat;
        }

        public async Task IssueRefreshTokenAsync(TokenGeneratingContext context)
        {
            var refreshToken = await CreateRefreshTokenAsync(context);
            var token = _dataFormat.Protect(refreshToken);
            context.AddToken(new TokenResult(refreshToken, token));
        }

        private async Task<RefreshToken> CreateRefreshTokenAsync(TokenGeneratingContext context)
        {
            await _claimsManager.CreateClaimsAsync(context);

            var claims = context.CurrentClaims;

            return new RefreshToken(claims);
        }

        public Task<AuthorizationGrant> ExchangeRefreshTokenAsync(OpenIdConnectMessage message)
        {
            var refreshToken = _dataFormat.Unprotect(message.RefreshToken);

            var resource = refreshToken.Resource;
            var scopes = refreshToken.Scopes
                .Select(s => ApplicationScope.CanonicalScopes.TryGetValue(s, out var scope) ? scope : new ApplicationScope(resource, s));

            return Task.FromResult(AuthorizationGrant.Valid(
                refreshToken.UserId,
                refreshToken.ClientId,
                refreshToken.GrantedTokens,
                scopes,
                refreshToken));
        }
    }
}

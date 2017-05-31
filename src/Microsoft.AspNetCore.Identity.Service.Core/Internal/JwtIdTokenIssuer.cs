// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.AspNetCore.Identity.Service.Issuers;
using Microsoft.AspNetCore.Identity.Service.Signing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class JwtIdTokenIssuer : IIdTokenIssuer
    {
        private static readonly string[] ClaimsToFilter = new string[]
        {
            TokenClaimTypes.TokenUniqueId,
            TokenClaimTypes.Issuer,
            TokenClaimTypes.Audience,
            TokenClaimTypes.IssuedAt,
            TokenClaimTypes.Expires,
            TokenClaimTypes.NotBefore,
            TokenClaimTypes.Nonce,
            TokenClaimTypes.CodeHash,
            TokenClaimTypes.AccessTokenHash,
        };

        private readonly ITokenClaimsManager _claimsManager;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly ApplicationTokenOptions _options;
        private readonly ISigningCredentialsPolicyProvider _credentialsProvider;

        public JwtIdTokenIssuer(
            ITokenClaimsManager claimsManager,
            ISigningCredentialsPolicyProvider credentialsProvider,
            JwtSecurityTokenHandler handler,
            IOptions<ApplicationTokenOptions> options)
        {
            _claimsManager = claimsManager;
            _credentialsProvider = credentialsProvider;
            _handler = handler;
            _options = options.Value;
        }

        public async Task IssueIdTokenAsync(TokenGeneratingContext context)
        {
            var idToken = await CreateIdTokenAsync(context);
            var subjectIdentity = CreateSubject(idToken);

            var descriptor = new SecurityTokenDescriptor();

            descriptor.Issuer = idToken.Issuer;
            descriptor.Audience = idToken.Audience;
            descriptor.Subject = subjectIdentity;
            descriptor.IssuedAt = idToken.IssuedAt.UtcDateTime;
            descriptor.Expires = idToken.Expires.UtcDateTime;
            descriptor.NotBefore = idToken.NotBefore.UtcDateTime;

            var credentialsDescriptor = await _credentialsProvider.GetSigningCredentialsAsync();
            descriptor.SigningCredentials = credentialsDescriptor.Credentials;

            var token = _handler.CreateJwtSecurityToken(descriptor);

            token.Payload.Remove(TokenClaimTypes.JwtId);

            if (idToken.Nonce != null)
            {
                token.Payload.AddClaim(new Claim(TokenClaimTypes.Nonce, idToken.Nonce));
            }

            if (idToken.CodeHash != null)
            {
                token.Payload.AddClaim(new Claim(TokenClaimTypes.CodeHash, idToken.CodeHash));
            }

            if (idToken.AccessTokenHash != null)
            {
                token.Payload.AddClaim(new Claim(TokenClaimTypes.AccessTokenHash, idToken.AccessTokenHash));
            }

            context.AddToken(new TokenResult(idToken, _handler.WriteToken(token)));
        }

        private ClaimsIdentity CreateSubject(IdToken idToken) =>
            new ClaimsIdentity(GetFilteredClaims(idToken));

        private IEnumerable<Claim> GetFilteredClaims(IdToken token)
        {
            foreach (var claim in token)
            {
                if (!ClaimsToFilter.Contains(claim.Type))
                {
                    yield return claim;
                }
            }
        }

        private async Task<IdToken> CreateIdTokenAsync(TokenGeneratingContext context)
        {
            await _claimsManager.CreateClaimsAsync(context);
            
            var claims = context.CurrentClaims;

            return new IdToken(claims);
        }
    }
}

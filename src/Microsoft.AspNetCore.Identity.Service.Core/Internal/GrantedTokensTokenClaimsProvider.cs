// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service.Claims;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class GrantedTokensTokenClaimsProvider : ITokenClaimsProvider
    {
        public int Order => 100;

        public Task OnGeneratingClaims(TokenGeneratingContext context)
        {
            if (context.IsContextForTokenTypes(TokenTypes.AuthorizationCode))
            {
                foreach (var grantedToken in GetGrantedTokensForAuthorizationCode(context))
                {
                    context.AddClaimToCurrentToken(TokenClaimTypes.GrantedToken, grantedToken);
                }
            }

            if (context.IsContextForTokenTypes(TokenTypes.RefreshToken))
            {
                foreach (var grantedToken in context.RequestGrants.Tokens)
                {
                    context.AddClaimToCurrentToken(TokenClaimTypes.GrantedToken, grantedToken);
                }
            }

            return Task.CompletedTask;
        }

        private IEnumerable<string> GetGrantedTokensForAuthorizationCode(TokenGeneratingContext context)
        {
            if (context.RequestGrants.Scopes.Any(s => s.ClientId != null))
            {
                yield return TokenTypes.AccessToken;
            }

            if (context.RequestGrants.Scopes.Contains(ApplicationScope.OpenId))
            {
                yield return TokenTypes.IdToken;
            }

            if (context.RequestGrants.Scopes.Contains(ApplicationScope.OfflineAccess))
            {
                yield return TokenTypes.RefreshToken;
            }
        }
    }
}

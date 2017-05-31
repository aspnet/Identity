// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service.Claims;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class ScopesTokenClaimsProvider : ITokenClaimsProvider
    {
        public int Order => 100;

        public Task OnGeneratingClaims(TokenGeneratingContext context)
        {
            var resource = context.RequestGrants.Scopes.FirstOrDefault(rg => rg.ClientId != null)?.ClientId;

            if (context.IsContextForTokenTypes(TokenTypes.AccessToken) && resource != null)
            {
                // For access tokens we use the scopes from the set of granted scopes, this takes into account the
                // fact that a token request can ask for a subset of the scopes granted during authorization, either
                // on a code exchange or on a refresh token grant flow.
                AddClaimsForAccessToken(context, resource);
                return Task.CompletedTask;
            }

            if (context.IsContextForTokenTypes(TokenTypes.AuthorizationCode))
            {
                context.AddClaimToCurrentToken(
                    TokenClaimTypes.Scope,
                    GetScopeValue(context.RequestGrants.Scopes, excludeCanonical: false));

                if (resource != null)
                {
                    context.AddClaimToCurrentToken(TokenClaimTypes.Resource, resource);
                }

                return Task.CompletedTask;
            }


            if (context.IsContextForTokenTypes(TokenTypes.RefreshToken))
            {
                // For refresh tokens the scope claim never changes as the set of scopes granted for a refresh token
                // should not change no matter what scopes are sent on a token request.
                var scopeClaim = context
                    .RequestGrants
                    .Claims
                    .Single(c => c.Type.Equals(TokenClaimTypes.Scope, StringComparison.Ordinal));

                var resourceClaim = context
                    .RequestGrants
                    .Claims
                    .SingleOrDefault(c => c.Type.Equals(TokenClaimTypes.Resource, StringComparison.Ordinal));

                context.AddClaimToCurrentToken(scopeClaim);

                if (resourceClaim != null)
                {
                    context.AddClaimToCurrentToken(resourceClaim);
                }
            }

            return Task.CompletedTask;
        }

        private void AddClaimsForAccessToken(TokenGeneratingContext context, string resource)
        {
            var scopes = context.RequestGrants.Scopes;
            var accessTokenScopes = GetAccessTokenScopes(scopes);

            context.AddClaimToCurrentToken(TokenClaimTypes.Scope, GetScopeValue(scopes, excludeCanonical: true));
            context.AddClaimToCurrentToken(TokenClaimTypes.Audience, resource);
            context.AddClaimToCurrentToken(TokenClaimTypes.AuthorizedParty, context.RequestParameters.ClientId);
        }

        private IEnumerable<ApplicationScope> GetAccessTokenScopes(IEnumerable<ApplicationScope> applicationScopes) =>
            applicationScopes.Where(s => s.ClientId != null);

        private string GetScopeValue(IEnumerable<ApplicationScope> scopes, bool excludeCanonical) =>
            !excludeCanonical ?
                string.Join(" ", scopes.Select(s => s.Scope)) :
                string.Join(" ", scopes.Where(s => s.ClientId != null).Select(s => s.Scope));
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class NonceTokenClaimsProvider : ITokenClaimsProvider
    {
        public int Order => 100;

        public Task OnGeneratingClaims(TokenGeneratingContext context)
        {
            var nonce = GetNonce(context);
            if (context.IsContextForTokenTypes(
                TokenTypes.IdToken,
                TokenTypes.AccessToken,
                TokenTypes.AuthorizationCode) && nonce != null)
            {
                context.AddClaimToCurrentToken(TokenClaimTypes.Nonce, nonce);
            }

            return Task.CompletedTask;
        }

        private string GetNonce(TokenGeneratingContext context) =>
            context.RequestParameters.RequestType == OpenIdConnectRequestType.Authentication ?
                context.RequestParameters.Nonce :
                context.RequestGrants.Claims.SingleOrDefault(c => c.Type.Equals(TokenClaimTypes.Nonce))?.Value;
    }
}

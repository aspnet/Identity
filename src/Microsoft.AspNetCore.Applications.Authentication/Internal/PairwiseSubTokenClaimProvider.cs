// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.Applications.Authentication.Internal
{
    /// <summary>
    /// Produces a <c>sub</c> claim that is unique per user and application.
    /// </summary>
    public class PairwiseSubClaimProvider : ITokenClaimsProvider
    {
        private readonly IdentityOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="PairwiseSubClaimProvider"/>.
        /// </summary>
        /// <param name="options"></param>
        public PairwiseSubClaimProvider(IOptions<IdentityOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc />
        public int Order => 200;

        /// <inheritdoc />
        public Task OnGeneratingClaims(TokenGeneratingContext context)
        {
            if (context.CurrentToken.Equals(TokenTypes.IdToken) ||
               context.CurrentToken.Equals(TokenTypes.AccessToken))
            {
                var userId = context.User.FindFirstValue(_options.ClaimsIdentity.UserIdClaimType);
                var applicationId = context.Application.FindFirstValue(TokenClaimTypes.ObjectId);
                var unHashedSubjectBits = Encoding.ASCII.GetBytes($"{userId}/{applicationId}");
                var hashing = CryptographyHelpers.CreateSHA256();
                var subject = Base64UrlEncoder.Encode(hashing.ComputeHash(unHashedSubjectBits));
                Claim existingClaim = null;
                foreach (var claim in context.CurrentClaims)
                {
                    if (claim.Type.Equals(TokenClaimTypes.Subject, StringComparison.Ordinal))
                    {
                        existingClaim = claim;
                    }
                }

                if (existingClaim != null)
                {
                    context.CurrentClaims.Remove(existingClaim);
                }

                context.CurrentClaims.Add(new Claim(TokenClaimTypes.Subject, subject));
            }

            return Task.CompletedTask;
        }
    }
}

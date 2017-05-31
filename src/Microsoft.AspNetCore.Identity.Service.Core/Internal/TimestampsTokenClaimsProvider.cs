// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.AspNetCore.Identity.Service.Validation;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class TimestampsTokenClaimsProvider : ITokenClaimsProvider
    {
        private readonly ITimeStampManager _timeStampManager;
        private readonly IOptions<ApplicationTokenOptions> _options;

        public TimestampsTokenClaimsProvider(
            ITimeStampManager timestampManager,
            IOptions<ApplicationTokenOptions> options)
        {
            _timeStampManager = timestampManager;
            _options = options;
        }

        public int Order => 100;

        public Task OnGeneratingClaims(TokenGeneratingContext context)
        {
            var options = GetOptions(context.CurrentToken);
            context.CurrentClaims.Add(new Claim(
                TokenClaimTypes.NotBefore,
                _timeStampManager.GetTimeStampInEpochTime(options.NotValidBefore)));

            context.CurrentClaims.Add(new Claim(
                TokenClaimTypes.IssuedAt,
                _timeStampManager.GetCurrentTimeStampInEpochTime()));

            context.CurrentClaims.Add(new Claim(
                TokenClaimTypes.Expires,
                _timeStampManager.GetTimeStampInEpochTime(options.NotValidAfter)));

            return Task.CompletedTask;
        }

        private TokenClaimsOptions GetOptions(string tokenType)
        {
            switch (tokenType)
            {
                case TokenTypes.AccessToken:
                    return _options.Value.AccessTokenOptions;
                case TokenTypes.AuthorizationCode:
                    return _options.Value.AuthorizationCodeOptions;
                case TokenTypes.IdToken:
                    return _options.Value.IdTokenOptions;
                case TokenTypes.RefreshToken:
                    return _options.Value.RefreshTokenOptions;
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task OnValidatingClaims(TokenGeneratingContext context)
        {
            return Task.CompletedTask;
        }
    }
}

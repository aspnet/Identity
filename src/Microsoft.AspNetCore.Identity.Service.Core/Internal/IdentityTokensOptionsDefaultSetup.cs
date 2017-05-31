// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class IdentityTokensOptionsDefaultSetup : IConfigureOptions<ApplicationTokenOptions>
    {
        public void Configure(ApplicationTokenOptions options)
        {
            options.SerializationSettings = CreateDefault();
            options.SerializationSettings.Converters.Insert(0, new AuthorizationCodeConverter());
            options.SerializationSettings.Converters.Insert(0, new RefreshTokenConverter());

            options.AuthorizationCodeOptions = CreateAuthorizationCodeOptions(TimeSpan.FromMinutes(5), TimeSpan.Zero);
            options.AccessTokenOptions = CreateAccessTokenOptions(TimeSpan.FromHours(2), TimeSpan.Zero);
            options.RefreshTokenOptions = CreateRefreshTokenOptions(TimeSpan.FromDays(30), TimeSpan.Zero);
            options.IdTokenOptions = CreateIdTokenOptions(TimeSpan.FromHours(2), TimeSpan.Zero);
        }

        private static TokenClaimsOptions CreateAuthorizationCodeOptions(TimeSpan notValidAfter, TimeSpan notValidBefore)
        {
            var userClaims = new TokenMapping("user");
            userClaims.AddSingle(TokenClaimTypes.UserId, ClaimTypes.NameIdentifier);

            var applicationClaims = new TokenMapping("application");
            applicationClaims.AddSingle(TokenClaimTypes.ClientId);

            return new TokenClaimsOptions()
            {
                UserClaims = userClaims,
                ApplicationClaims = applicationClaims,
                NotValidAfter = notValidAfter,
                NotValidBefore = notValidBefore
            };
        }

        private static TokenClaimsOptions CreateAccessTokenOptions(TimeSpan notValidAfter, TimeSpan notValidBefore)
        {
            var userClaims = new TokenMapping("user");
            userClaims.AddSingle(TokenClaimTypes.Subject, ClaimTypes.NameIdentifier);

            var applicationClaims = new TokenMapping("application");

            return new TokenClaimsOptions()
            {
                UserClaims = userClaims,
                ApplicationClaims = applicationClaims,
                NotValidAfter = notValidAfter,
                NotValidBefore = notValidBefore
            };
        }

        private static TokenClaimsOptions CreateRefreshTokenOptions(TimeSpan notValidAfter, TimeSpan notValidBefore)
        {
            var userClaims = new TokenMapping("user");
            userClaims.AddSingle(TokenClaimTypes.UserId, ClaimTypes.NameIdentifier);

            var applicationClaims = new TokenMapping("application");
            applicationClaims.AddSingle(TokenClaimTypes.ClientId, TokenClaimTypes.ClientId);

            return new TokenClaimsOptions()
            {
                UserClaims = userClaims,
                ApplicationClaims = applicationClaims,
                NotValidAfter = notValidAfter,
                NotValidBefore = notValidBefore
            };
        }

        private static TokenClaimsOptions CreateIdTokenOptions(TimeSpan notValidAfter, TimeSpan notValidBefore)
        {
            var userClaims = new TokenMapping("user");

            var applicationClaims = new TokenMapping("application");
            applicationClaims.AddSingle(TokenClaimTypes.Audience, TokenClaimTypes.ClientId);

            return new TokenClaimsOptions()
            {
                UserClaims = userClaims,
                ApplicationClaims = applicationClaims,
                NotValidAfter = notValidAfter,
                NotValidBefore = notValidBefore
            };
        }

        private static JsonSerializerSettings CreateDefault() =>
            new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,

                // Limit the object graph we'll consume to a fixed depth. This prevents stackoverflow exceptions
                // from deserialization errors that might occur from deeply nested objects.
                MaxDepth = 32,

                // Do not change this setting
                // Setting this to None prevents Json.NET from loading malicious, unsafe, or security-sensitive types
                TypeNameHandling = TypeNameHandling.None,
            };
    }
}

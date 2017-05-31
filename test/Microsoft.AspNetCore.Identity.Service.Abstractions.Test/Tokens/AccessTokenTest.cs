// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class AccessTokenTest
    {
        [Fact]
        public void CreateAccessToken_Fails_IfMissingIssuerClaim()
        {
            // Arrange
            var claims = new List<Claim>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfMultipleIssuerClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer,"issuer"),
                new Claim(TokenClaimTypes.Issuer,"issuer"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfMissingSubjectClaim()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Subject, "subject")
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfMultipleSubjectClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Subject, "subject")
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfMissingAudienceClaim()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereIsMoreThanOneAudienceClaim()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Audience, "audience2"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereIsNoScopeClaim()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereAreMultipleScopeClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.Scope, "write"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereIsNoAuthorizedPartyClaim()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "scope"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereAreMultipleAuthorizedPartyClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty2"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereIsNoTokenId()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereAreMultipletokenIdClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "id1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "id2"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereIsNoIssuedAt()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereAreMultipleIssuedAtClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt1"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt2"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereIsNoExpires()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereAreMultipleExpiresClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
                new Claim(TokenClaimTypes.Expires, "expires"),
                new Claim(TokenClaimTypes.Expires, "expires"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereIsNoNotBefore()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
                new Claim(TokenClaimTypes.Expires, "expires"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }

        [Fact]
        public void CreateAccessToken_Fails_IfThereAreMultipleNotBeforeClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.Issuer, "issuer"),
                new Claim(TokenClaimTypes.Subject, "subject"),
                new Claim(TokenClaimTypes.Audience, "audience1"),
                new Claim(TokenClaimTypes.Scope, "read"),
                new Claim(TokenClaimTypes.AuthorizedParty, "authorizedParty1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
                new Claim(TokenClaimTypes.Expires, "expires"),
                new Claim(TokenClaimTypes.NotBefore, "notBefore"),
                new Claim(TokenClaimTypes.NotBefore, "notBefore"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AccessToken(claims));
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class AuthorizationCodeTest
    {
        [Fact]
        public void CreateAuthorizationCode_Fails_IfMissingUserIdClaim()
        {
            // Arrange
            var claims = new List<Claim>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfMultipleUserClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId,"userId"),
                new Claim(TokenClaimTypes.UserId,"userId"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfMissingClientIdClaim()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId")
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfMultipleClientIdClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.ClientId, "clientId")
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereIsMoreThanOneRedirectUri()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri2"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereIsNoScopeClaim()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereAreMultipleScopeClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid profile"),
                new Claim(TokenClaimTypes.Scope, "offline_access"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereIsNoGrantedToken()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereIsNoTokenId()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereAreMultipletokenIdClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
                new Claim(TokenClaimTypes.TokenUniqueId, "id1"),
                new Claim(TokenClaimTypes.TokenUniqueId, "id2"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereIsNoIssuedAt()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereAreMultipleIssuedAtClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt1"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt2"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereIsNoExpires()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereAreMultipleExpiresClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
                new Claim(TokenClaimTypes.Expires, "expires"),
                new Claim(TokenClaimTypes.Expires, "expires"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereIsNoNotBefore()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
                new Claim(TokenClaimTypes.Expires, "expires"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }

        [Fact]
        public void CreateAuthorizationCode_Fails_IfThereAreMultipleNotBeforeClaims()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(TokenClaimTypes.UserId, "userId"),
                new Claim(TokenClaimTypes.ClientId, "clientId"),
                new Claim(TokenClaimTypes.RedirectUri, "redirectUri1"),
                new Claim(TokenClaimTypes.Scope, "openid"),
                new Claim(TokenClaimTypes.GrantedToken, "access_token"),
                new Claim(TokenClaimTypes.TokenUniqueId, "tuid"),
                new Claim(TokenClaimTypes.IssuedAt, "issuedAt"),
                new Claim(TokenClaimTypes.Expires, "expires"),
                new Claim(TokenClaimTypes.NotBefore, "notBefore"),
                new Claim(TokenClaimTypes.NotBefore, "notBefore"),
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new AuthorizationCode(claims));
        }
    }
}

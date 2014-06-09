// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class ClaimsIdentityFactoryTest
    {
        [Fact]
        public async Task CreateIdentityNullChecks()
        {
            var userManager = MockHelpers.MockUserManager<TestUser>().Object;
            var roleManager = MockHelpers.MockRoleManager<TestRole>().Object;
            var factory = new ClaimsIdentityFactory<TestUser, TestRole>(userManager, roleManager);
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                async () => await factory.CreateAsync(null, "whatever"));
            await Assert.ThrowsAsync<ArgumentNullException>("value",
                async () => await factory.CreateAsync(new TestUser(), null));
        }

 #if NET45
        //TODO: Mock fails in K (this works fine in net45)
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task EnsureClaimsIdentityHasExpectedClaims(bool supportRoles, bool supportClaims)
        {
            // Setup
            var userManager = MockHelpers.MockUserManager<TestUser>();
            var roleManager = MockHelpers.MockRoleManager<TestRole>();
            var user = new TestUser { UserName = "Foo" };
            userManager.Setup(m => m.SupportsUserRole).Returns(supportRoles);
            userManager.Setup(m => m.SupportsUserClaim).Returns(supportClaims);
            userManager.Setup(m => m.GetUserIdAsync(user, CancellationToken.None)).ReturnsAsync(user.Id);
            userManager.Setup(m => m.GetUserNameAsync(user, CancellationToken.None)).ReturnsAsync(user.UserName);
            var roleClaims = new[] { "Admin", "Local" }; 
            userManager.Setup(m => m.GetRolesAsync(user, CancellationToken.None)).ReturnsAsync(roleClaims);
            var userClaims = new[] { new Claim("Whatever", "Value"), new Claim("Whatever2", "Value2") };
            userManager.Setup(m => m.GetClaimsAsync(user, CancellationToken.None)).ReturnsAsync(userClaims);
            userManager.Object.Options = new IdentityOptions();

            const string authType = "Microsoft.AspNet.Identity";
            var factory = new ClaimsIdentityFactory<TestUser, TestRole>(userManager.Object, roleManager.Object);

            // Act
            var identity = await factory.CreateAsync(user, authType);

            // Assert
            var manager = userManager.Object;
            Assert.NotNull(identity);
            Assert.Equal(authType, identity.AuthenticationType);
            var claims = identity.Claims.ToList();
            Assert.NotNull(claims);
            Assert.True(
                claims.Any(c => c.Type == manager.Options.ClaimType.UserName && c.Value == user.UserName));
            Assert.True(claims.Any(c => c.Type == manager.Options.ClaimType.UserId && c.Value == user.Id));
            Assert.Equal(supportRoles, claims.Any(c => c.Type == manager.Options.ClaimType.Role && c.Value == "Admin"));
            Assert.Equal(supportRoles, claims.Any(c => c.Type == manager.Options.ClaimType.Role && c.Value == "Local"));
            foreach (var cl in userClaims)
            {
                Assert.Equal(supportClaims, claims.Any(c => c.Type == cl.Type && c.Value == cl.Value));
            }
        }
#endif
    }
}
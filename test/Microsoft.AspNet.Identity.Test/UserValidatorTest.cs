// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class UserValidatorTest
    {
        [Fact]
        public async Task ValidateThrowsWithNull()
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            var validator = new UserValidator<TestUser>();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>("manager", () => validator.ValidateAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => validator.ValidateAsync(manager, null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ValidateFailsWithTooShortUserNames(string input)
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            var validator = new UserValidator<TestUser>();
            var user = new TestUser {UserName = input};

            // Act
            var result = await validator.ValidateAsync(manager, user);

            // Assert
            IdentityResultAssert.IsFailure(result, "UserName cannot be null or empty.");
        }

        [Theory]
        [InlineData("test_email@foo.com", true)]
        [InlineData("hao", true)]
        [InlineData("test123", true)]
        [InlineData("!noway", false)]
        [InlineData("foo@boz#.com", false)]
        public async Task DefaultAlphaNumericOnlyUserNameValidation(string userName, bool expectSuccess)
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            var validator = new UserValidator<TestUser>();
            var user = new TestUser {UserName = userName};

            // Act
            var result = await validator.ValidateAsync(manager, user);

            // Assert
            if (expectSuccess)
            {
                IdentityResultAssert.IsSuccess(result);
            }
            else
            {
                IdentityResultAssert.IsFailure(result);
            }
        }

        [Theory]
        [InlineData("test_email@foo.com", true)]
        [InlineData("hao", true)]
        [InlineData("test123", true)]
        [InlineData("!noway", true)]
        [InlineData("foo@boz#.com", true)]
        public async Task CanAllowNonAlphaNumericUserName(string userName, bool expectSuccess)
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            manager.Options.User.AllowOnlyAlphanumericNames = false;
            var validator = new UserValidator<TestUser>();
            var user = new TestUser {UserName = userName};

            // Act
            var result = await validator.ValidateAsync(manager, user);

            // Assert
            if (expectSuccess)
            {
                IdentityResultAssert.IsSuccess(result);
            }
            else
            {
                IdentityResultAssert.IsFailure(result);
            }
        }
    }
}
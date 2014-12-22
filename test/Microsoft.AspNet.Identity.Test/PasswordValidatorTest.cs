// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class PasswordValidatorTest
    {
        [Flags]
        public enum Errors
        {
            None = 0,
            Length = 2,
            Alpha = 4,
            Upper = 8,
            Lower = 16,
            Digit = 32,
        }

        [Fact]
        public async Task ValidateThrowsWithNullTest()
        {
            // Setup
            var validator = new PasswordValidator<IdentityUser>();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>("password", () => validator.ValidateAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("manager", () => validator.ValidateAsync(null, null, "foo"));
        }


        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("abcde")]
        public async Task FailsIfTooShortTests(string input)
        {
            var manager = MockHelpers.TestUserManager<IdentityUser>();
            var valid = new PasswordValidator<IdentityUser>();
            manager.Options.Password.RequireUppercase = false;
            manager.Options.Password.RequireNonLetterOrDigit = false;
            manager.Options.Password.RequireLowercase = false;
            manager.Options.Password.RequireDigit = false;
            IdentityResultAssert.IsFailure(await valid.ValidateAsync(input, manager), IdentityFailure.PasswordTooShort);
        }

        [Theory]
        [InlineData("abcdef")]
        [InlineData("aaaaaaaaaaa")]
        public async Task SuccessIfLongEnoughTests(string input)
        {
            var manager = MockHelpers.TestUserManager<IdentityUser>();
            var valid = new PasswordValidator<IdentityUser>();
            manager.Options.Password.RequireUppercase = false;
            manager.Options.Password.RequireNonLetterOrDigit = false;
            manager.Options.Password.RequireLowercase = false;
            manager.Options.Password.RequireDigit = false;
            IdentityResultAssert.IsSuccess(await valid.ValidateAsync(manager, null, input));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aaaaaaaaaaa")]
        public async Task FailsWithoutRequiredNonAlphanumericTests(string input)
        {
            var manager = MockHelpers.TestUserManager<IdentityUser>();
            var valid = new PasswordValidator<IdentityUser>();
            manager.Options.Password.RequireUppercase = false;
            manager.Options.Password.RequireNonLetterOrDigit = true;
            manager.Options.Password.RequireLowercase = false;
            manager.Options.Password.RequireDigit = false;
            manager.Options.Password.RequiredLength = 0;
            IdentityResultAssert.IsFailure(await valid.ValidateAsync(input, manager), IdentityFailure.PasswordRequiresNonLetterAndDigit);
        }

        [Theory]
        [InlineData("@")]
        [InlineData("abcd@e!ld!kajfd")]
        [InlineData("!!!!!!")]
        public async Task SucceedsWithRequiredNonAlphanumericTests(string input)
        {
            var manager = MockHelpers.TestUserManager<IdentityUser>();
            var valid = new PasswordValidator<IdentityUser>();
            manager.Options.Password.RequireUppercase = false;
            manager.Options.Password.RequireNonLetterOrDigit = true;
            manager.Options.Password.RequireLowercase = false;
            manager.Options.Password.RequireDigit = false;
            manager.Options.Password.RequiredLength = 0;
            IdentityResultAssert.IsSuccess(await valid.ValidateAsync(manager, null, input));
        }

        [Theory]
        [InlineData("abcde", Errors.Length | Errors.Alpha | Errors.Upper | Errors.Digit)]
        [InlineData("a@B@cd", Errors.Digit)]
        [InlineData("___", Errors.Length | Errors.Digit | Errors.Lower | Errors.Upper)]
        [InlineData("a_b9de", Errors.Upper)]
        [InlineData("abcd@e!ld!kaj9Fd", Errors.None)]
        [InlineData("aB1@df", Errors.None)]
        public async Task UberMixedRequiredTests(string input, Errors errorMask)
        {
            var manager = MockHelpers.TestUserManager<IdentityUser>();
            var valid = new PasswordValidator<IdentityUser>();
            if (errorMask == Errors.None)
            {
                IdentityResultAssert.IsSuccess(await valid.ValidateAsync(manager, null, input));
            }
            else
            {
                var result = await valid.ValidateAsync(input, manager);
                IdentityResultAssert.IsFailure(result);
                if ((errorMask & Errors.Length) != Errors.None)
                {
                    Assert.True(result.Failures.Contains(IdentityFailure.PasswordTooShort));
                }
                if ((errorMask & Errors.Alpha) != Errors.None)
                {
                    Assert.True(result.Failures.Contains(IdentityFailure.PasswordRequiresNonLetterAndDigit));
                }
                if ((errorMask & Errors.Digit) != Errors.None)
                {
                    Assert.True(result.Failures.Contains(IdentityFailure.PasswordRequiresDigit));
                }
                if ((errorMask & Errors.Lower) != Errors.None)
                {
                    Assert.True(result.Failures.Contains(IdentityFailure.PasswordRequiresLower));
                }
                if ((errorMask & Errors.Upper) != Errors.None)
                {
                    Assert.True(result.Failures.Contains(IdentityFailure.PasswordRequiresUpper));
                }
            }
        }
    }
}
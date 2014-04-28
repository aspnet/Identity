using System;
using System.Security.Claims;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class IdentityOptionsTest
    {
        [Fact]
        public void VerifyDefaultOptions()
        {
            var options = new IdentityOptions();
            Assert.False(options.LockoutEnabledByDefault);
            Assert.Equal(TimeSpan.FromMinutes(5), options.LockoutDefaultTimeSpan);
            Assert.Equal(5, options.LockoutMaxFailedAccessAttempts);

            Assert.True(options.PasswordsRequireDigit);
            Assert.True(options.PasswordsRequireLowercase);
            Assert.True(options.PasswordsRequireNonLetterOrDigit);
            Assert.True(options.PasswordsRequireUppercase);
            Assert.Equal(6, options.PasswordsRequiredLength);

            Assert.True(options.UsersAllowOnlyAlphanumericNames);
            Assert.False(options.UsersRequireUniqueEmail);

            Assert.Equal(ClaimTypes.Role, options.ClaimTypeRole);
            Assert.Equal(ClaimTypes.Name, options.ClaimTypeUserName);
            Assert.Equal(ClaimTypes.NameIdentifier, options.ClaimTypeUserId);
            Assert.Equal(IdentityOptions.DefaultSecurityStampClaimType, options.ClaimTypeSecurityStamp);
        }

        [Fact]
        public void CopyNullIsNoop()
        {
            var options = new IdentityOptions();
            options.Copy(null);
        }
    }
}
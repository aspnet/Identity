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
            Assert.False(options.Lockout.EnabledByDefault);
            Assert.Equal(TimeSpan.FromMinutes(5), options.Lockout.DefaultLockoutTimeSpan);
            Assert.Equal(5, options.Lockout.MaxFailedAccessAttempts);

            Assert.True(options.Password.RequireDigit);
            Assert.True(options.Password.RequireLowercase);
            Assert.True(options.Password.RequireNonLetterOrDigit);
            Assert.True(options.Password.RequireUppercase);
            Assert.Equal(6, options.Password.RequiredLength);

            Assert.True(options.User.AllowOnlyAlphanumericNames);
            Assert.False(options.User.RequireUniqueEmail);

            Assert.Equal(ClaimTypes.Role, options.ClaimType.Role);
            Assert.Equal(ClaimTypes.Name, options.ClaimType.UserName);
            Assert.Equal(ClaimTypes.NameIdentifier, options.ClaimType.UserId);
            Assert.Equal(ClaimTypeOptions.DefaultSecurityStampClaimType, options.ClaimType.SecurityStamp);
        }

        [Fact]
        public void CopyNullIsNoop()
        {
            var options = new IdentityOptions();
            options.Copy(null);
        }
    }
}
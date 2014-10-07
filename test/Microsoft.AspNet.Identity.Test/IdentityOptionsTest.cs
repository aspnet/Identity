// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;
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

            Assert.Equal("^[a-zA-Z0-9@_\\.]+$", options.User.UserNameValidationRegex);
            Assert.False(options.User.RequireUniqueEmail);

            Assert.Equal(ClaimTypes.Role, options.ClaimsIdentity.RoleClaimType);
            Assert.Equal(ClaimTypes.Name, options.ClaimsIdentity.UserNameClaimType);
            Assert.Equal(ClaimTypes.NameIdentifier, options.ClaimsIdentity.UserIdClaimType);
            Assert.Equal(ClaimsIdentityOptions.DefaultSecurityStampClaimType, options.ClaimsIdentity.SecurityStampClaimType);
        }

        [Fact]
        public void IdentityOptionsFromConfig()
        {
            const string roleClaimType = "rolez";
            const string usernameClaimType = "namez";
            const string useridClaimType = "idz";
            const string securityStampClaimType = "stampz";

            var dic = new Dictionary<string, string>
            {
                {"identity:claimsidentity:roleclaimtype", roleClaimType},
                {"identity:claimsidentity:usernameclaimtype", usernameClaimType},
                {"identity:claimsidentity:useridclaimtype", useridClaimType},
                {"identity:claimsidentity:securitystampclaimtype", securityStampClaimType},
                {"identity:user:requireUniqueEmail", "true"},
                {"identity:password:RequiredLength", "10"},
                {"identity:password:RequireNonLetterOrDigit", "false"},
                {"identity:password:RequireUpperCase", "false"},
                {"identity:password:RequireDigit", "false"},
                {"identity:password:RequireLowerCase", "false"},
                {"identity:lockout:EnabledByDefault", "TRUe"},
                {"identity:lockout:MaxFailedAccessAttempts", "1000"}
            };
            var config = new Configuration { new MemoryConfigurationSource(dic) };
            Assert.Equal(roleClaimType, config.Get("identity:claimsidentity:roleclaimtype"));

            var services = new ServiceCollection {OptionsServices.GetDefaultServices()};
            services.AddIdentity(config.GetSubKey("identity"));
            var accessor = services.BuildServiceProvider().GetService<IOptionsAccessor<IdentityOptions>>();
            Assert.NotNull(accessor);
            var options = accessor.Options;
            Assert.Equal(roleClaimType, options.ClaimsIdentity.RoleClaimType);
            Assert.Equal(useridClaimType, options.ClaimsIdentity.UserIdClaimType);
            Assert.Equal(usernameClaimType, options.ClaimsIdentity.UserNameClaimType);
            Assert.Equal(securityStampClaimType, options.ClaimsIdentity.SecurityStampClaimType);
            Assert.True(options.User.RequireUniqueEmail);
            Assert.Equal("^[a-zA-Z0-9@_\\.]+$", options.User.UserNameValidationRegex);
            Assert.False(options.Password.RequireDigit);
            Assert.False(options.Password.RequireLowercase);
            Assert.False(options.Password.RequireNonLetterOrDigit);
            Assert.False(options.Password.RequireUppercase);
            Assert.Equal(10, options.Password.RequiredLength);
            Assert.True(options.Lockout.EnabledByDefault);
            Assert.Equal(1000, options.Lockout.MaxFailedAccessAttempts);
        }

        public class PasswordsNegativeLengthSetup : ConfigureOptions<IdentityOptions>
        {
            public PasswordsNegativeLengthSetup() 
                : base(options => options.Password.RequiredLength = -1)
            { }
        }

        [Fact]
        public void CanCustomizeIdentityOptions()
        {
            var builder = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
            builder.UseServices(services =>
            {
                services.AddIdentity<IdentityUser>();
                services.AddConfigureOptions<PasswordsNegativeLengthSetup>();
            });

            var setup = builder.ApplicationServices.GetService<IConfigureOptions<IdentityOptions>>();
            Assert.IsType(typeof(PasswordsNegativeLengthSetup), setup);
            var optionsGetter = builder.ApplicationServices.GetService<IOptionsAccessor<IdentityOptions>>();
            Assert.NotNull(optionsGetter);
            var myOptions = optionsGetter.Options;
            Assert.True(myOptions.Password.RequireLowercase);
            Assert.True(myOptions.Password.RequireDigit);
            Assert.True(myOptions.Password.RequireNonLetterOrDigit);
            Assert.True(myOptions.Password.RequireUppercase);
            Assert.Equal(-1, myOptions.Password.RequiredLength);
        }

        [Fact]
        public void CanSetupIdentityOptions()
        {
            var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
            app.UseServices(services =>
            {
                services.AddIdentity<IdentityUser>().ConfigureIdentity(options => options.User.RequireUniqueEmail = true);
            });

            var optionsGetter = app.ApplicationServices.GetService<IOptionsAccessor<IdentityOptions>>();
            Assert.NotNull(optionsGetter);

            var myOptions = optionsGetter.Options;
            Assert.True(myOptions.User.RequireUniqueEmail);
        }

    }
}
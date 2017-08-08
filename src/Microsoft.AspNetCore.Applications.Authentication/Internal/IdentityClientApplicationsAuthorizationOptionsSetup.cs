// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Applications.Authentication.Internal
{
    /// <summary>
    /// Setup for <see cref="AuthorizationOptions"/>.
    /// </summary>
    public class IdentityClientApplicationsAuthorizationOptionsSetup : IConfigureOptions<AuthorizationOptions>
    {
        private readonly IOptions<ApplicationTokenOptions> tokenOptions;

        /// <summary>
        /// Creates a new instance of <see cref="IdentityClientApplicationsAuthorizationOptionsSetup"/>.
        /// </summary>
        /// <param name="tokenOptions"></param>
        public IdentityClientApplicationsAuthorizationOptionsSetup(IOptions<ApplicationTokenOptions> tokenOptions)
        {
            this.tokenOptions = tokenOptions;
        }

        /// <inheritdoc />
        public void Configure(AuthorizationOptions options)
        {
            var loginPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                .RequireAuthenticatedUser()
                .Build();

            var sessionPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                .AddAuthenticationSchemes(ApplicationsAuthenticationDefaults.CookieAuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            var managementPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                .AddAuthenticationSchemes(ApplicationsAuthenticationDefaults.CookieAuthenticationScheme)
                .AddRequirements(new ApplicationManagementRequirement())
                .Build();

            options.AddPolicy(ApplicationsAuthenticationDefaults.LoginPolicyName, loginPolicy);
            options.AddPolicy(ApplicationsAuthenticationDefaults.SessionPolicyName, sessionPolicy);
            options.AddPolicy(ApplicationsAuthenticationDefaults.ManagementPolicyName, managementPolicy);
        }
    }
}

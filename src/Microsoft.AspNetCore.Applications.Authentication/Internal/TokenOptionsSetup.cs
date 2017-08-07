// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Applications.Authentication.Internal
{
    /// <summary>
    /// Setup <see cref="IOptions{ApplicationTokenOptions}"/>.
    /// </summary>
    public class TokenOptionsSetup : IConfigureOptions<ApplicationTokenOptions>
    {
        private readonly IOptions<IdentityOptions> _options;

        /// <summary>
        /// Creates a new instance of <see cref="TokenOptionsSetup"/>.
        /// </summary>
        /// <param name="options"></param>
        public TokenOptionsSetup(IOptions<IdentityOptions> options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public void Configure(ApplicationTokenOptions options)
        {
            options.IdTokenOptions.UserClaims
                .AddSingle(TokenClaimTypes.Subject, _options.Value.ClaimsIdentity.UserIdClaimType);

            options.IdTokenOptions.UserClaims
                .AddSingle(TokenClaimTypes.Name, _options.Value.ClaimsIdentity.UserNameClaimType);

            options.AccessTokenOptions.UserClaims
                .AddSingle(TokenClaimTypes.Name, _options.Value.ClaimsIdentity.UserNameClaimType);
        }
    }
}

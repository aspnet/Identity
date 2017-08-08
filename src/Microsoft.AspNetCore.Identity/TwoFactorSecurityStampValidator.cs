// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides default implementation of validation functions for security stamps.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public class TwoFactorSecurityStampValidator<TUser> : ITwoFactorSecurityStampValidator where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly SecurityStampValidatorOptions _options;
        private ISystemClock _clock;

        /// <summary>
        /// Creates a new instance of <see cref="SecurityStampValidator{TUser}"/>.
        /// </summary>
        /// <param name="options">Used to access the <see cref="IdentityOptions"/>.</param>
        /// <param name="signInManager">The <see cref="SignInManager{TUser}"/>.</param>
        /// <param name="clock">The system clock.</param>
        public TwoFactorSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<TUser> signInManager, ISystemClock clock)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (signInManager == null)
            {
                throw new ArgumentNullException(nameof(signInManager));
            }
            _signInManager = signInManager;
            _options = options.Value;
            _clock = clock;
        }

        /// <summary>
        /// Validates a security stamp of an identity as an asynchronous operation, and rebuilds the identity if the validation succeeds, otherwise rejects
        /// the identity.
        /// </summary>
        /// <param name="context">The context containing the <see cref="System.Security.Claims.ClaimsPrincipal"/>
        /// and <see cref="Http.Authentication.AuthenticationProperties"/> to validate.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous validation operation.</returns>
        public virtual async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            var currentUtc = DateTimeOffset.UtcNow;
            if (context.Options != null && _clock != null)
            {
                currentUtc = _clock.UtcNow;
            }
            var issuedUtc = context.Properties.IssuedUtc;
            var userId = context?.Principal?.Identity?.Name;

            // Only validate we have a userId and if enough time has elapsed and security stamp is enabled
            var validate = userId != null && issuedUtc == null && _signInManager.UserManager.SupportsUserSecurityStamp;
            if (issuedUtc != null)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                validate = timeElapsed > _options.ValidationInterval;
            }
            if (validate)
            {
                var user = await _signInManager.UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var securityStamp =
                        context.Principal.FindFirstValue(_signInManager.Options.ClaimsIdentity.SecurityStampClaimType);
                    if (securityStamp == await _signInManager.UserManager.GetSecurityStampAsync(user))
                    {
                        // Security stamp matches, success.
                        return;
                    }
                }

                // Reject this cookie, and clear all the other cookies
                context.RejectPrincipal();
                await _signInManager.SignOutAsync();
            }
        }
    }
}
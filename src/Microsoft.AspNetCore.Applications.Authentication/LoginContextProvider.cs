// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Applications.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Service.Session;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class LoginContextProvider : ILoginContextProvider
    {
        private readonly ILoginFactory _loginFactory;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly IHttpContextAccessor _contextAccessor;
        protected readonly ProtocolErrorProvider _errorProvider;

        private HttpContext _context;

        public LoginContextProvider(
            ILoginFactory loginFactory,
            IAuthorizationPolicyProvider policyProvider,
            IHttpContextAccessor contextAccessor,
            ProtocolErrorProvider errorProvider)
        {
            _loginFactory = loginFactory;
            _policyProvider = policyProvider;
            _contextAccessor = contextAccessor;
            _errorProvider = errorProvider;
        }

        public HttpContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = _contextAccessor.HttpContext;
                }
                if (_context == null)
                {
                    throw new InvalidOperationException($"{nameof(HttpContext)} can't be null.");
                }

                return _context;
            }
            set
            {
                _context = value;
            }
        }

        private async Task<ClaimsPrincipal> GetPrincipal(string policyName)
        {
            var policy = await _policyProvider.GetPolicyAsync(policyName);
            ClaimsPrincipal newPrincipal = null;
            for (var i = 0; i < policy.AuthenticationSchemes.Count; i++)
            {
                var scheme = policy.AuthenticationSchemes[i];
                var result = await Context.AuthenticateAsync(scheme);
                if (result != null)
                {
                    newPrincipal = SecurityHelper.MergeUserPrincipal(newPrincipal, result.Principal);
                }
            }

            return newPrincipal;
        }

        public async Task<LoginContext> GetLoginContextAsync()
        {
            var loginPrincipal = await GetPrincipal(ApplicationsAuthenticationDefaults.LoginPolicyName);
            var loginSchemes = loginPrincipal.Identities.Select(i => i.AuthenticationType);
            var applicationsPrincipal = await GetPrincipal(ApplicationsAuthenticationDefaults.SessionPolicyName);
            var appIdentities = applicationsPrincipal.Identities
                .Where(i => !loginSchemes.Contains(i.AuthenticationType, StringComparer.Ordinal));

            return new LoginContext(loginPrincipal, new ClaimsPrincipal(appIdentities));
        }

        public async Task LogInAsync(ClaimsPrincipal user, ClaimsPrincipal application)
        {
            var context = await GetLoginContextAsync();
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationClientId = application.FindFirstValue(TokenClaimTypes.ClientId);

            var applications = new ClaimsPrincipal(application.Identities
                .Where(i => i.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId)
                .ToList());

            if (applications.FindFirstValue(applicationClientId) == null)
            {
                applications.AddIdentity(CreateApplicationIdentity(user, application));
            }

            var policy = await _policyProvider.GetPolicyAsync(ApplicationsAuthenticationDefaults.SessionPolicyName);
            var loginPolicy = await _policyProvider.GetPolicyAsync(ApplicationsAuthenticationDefaults.LoginPolicyName);
            for (var i = 0; i < policy.AuthenticationSchemes.Count; i++)
            {
                var scheme = policy.AuthenticationSchemes[i];
                if (loginPolicy.AuthenticationSchemes.Contains(scheme))
                {
                    continue;
                }

                await Context.SignInAsync(scheme, applications);
            }
        }

        private ClaimsIdentity CreateApplicationIdentity(ClaimsPrincipal user, ClaimsPrincipal application)
        {
            var identity = new ClaimsIdentity(ApplicationsAuthenticationDefaults.CookieAuthenticationScheme);
            var userId = user.FindFirst(ClaimTypes.NameIdentifier);
            var clientId = application.FindFirst(TokenClaimTypes.ClientId);
            identity.AddClaim(userId);
            identity.AddClaim(clientId);
            identity.AddClaims(application.FindAll(TokenClaimTypes.LogoutRedirectUri));

            return identity;
        }

        public async Task LogOutAsync(ClaimsPrincipal user, ClaimsPrincipal application)
        {
            var policy = await _policyProvider.GetPolicyAsync(ApplicationsAuthenticationDefaults.SessionPolicyName);
            for (var i = 0; i < policy.AuthenticationSchemes.Count; i++)
            {
                var scheme = policy.AuthenticationSchemes[i];
                await Context.SignOutAsync(scheme);
            }
        }
    }
}

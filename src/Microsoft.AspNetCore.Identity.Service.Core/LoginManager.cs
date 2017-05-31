// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Session;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.Applications.Authentication
{
    public class LoginManager
    {
        private readonly ILoginContextProvider _loginProvider;
        private readonly ILoginFactory _loginFactory;
        private readonly ProtocolErrorProvider _errorProvider;

        public LoginManager(
            ILoginContextProvider loginProvider,
            ILoginFactory loginFactory,
            ProtocolErrorProvider errorProvider)
        {
            _loginProvider = loginProvider;
            _loginFactory = loginFactory;
            _errorProvider = errorProvider;
        }

        public async Task<LoginResult> CanLogIn(AuthorizationRequest request)
        {
            var message = request.Message;
            var context = await _loginProvider.GetLoginContextAsync();
            var loggedUser = context.User;
            var sessions = context.Applications;

            var isLoggedIn = loggedUser.Identities.Any(i => i.IsAuthenticated);
            if (!isLoggedIn && PromptIsForbidden(message))
            {
                return LoginResult.Forbidden(RequiresLogin(request));
            }

            if (!isLoggedIn || PromptIsMandatory(message))
            {
                return LoginResult.LoginRequired();
            }

            return LoginResult.Authorized(
                await _loginFactory.GetUserAsync(loggedUser.FindFirst(ClaimTypes.NameIdentifier).Value),
                await _loginFactory.GetApplicationAsync(message.ClientId));
        }

        private bool PromptIsMandatory(OpenIdConnectMessage message)
        {
            return message.Prompt != null && message.Prompt.Contains(PromptValues.Login);
        }

        private bool PromptIsForbidden(OpenIdConnectMessage message)
        {
            return message.Prompt != null && message.Prompt.Contains(PromptValues.None);
        }

        private AuthorizationRequestError RequiresLogin(AuthorizationRequest request)
        {
            var error = _errorProvider.RequiresLogin();
            error.State = request.Message.State;

            return new AuthorizationRequestError(
                error,
                request.RequestGrants.RedirectUri,
                request.RequestGrants.ResponseMode);
        }

        public Task<LoginContext> GetLoginContextAsync()
        {
            return _loginProvider.GetLoginContextAsync();
        }

        public Task LogInAsync(ClaimsPrincipal user, ClaimsPrincipal application)
        {
            return _loginProvider.LogInAsync(user, application);
        }

        public async Task<LogoutResult> LogOutAsync(LogoutRequest request)
        {
            var context = await _loginProvider.GetLoginContextAsync();
            var application = request.LogoutRedirectUri != null ?
                context.Applications.Identities
                .FirstOrDefault(i => i.FindFirst(c => c.Type.Equals(TokenClaimTypes.LogoutRedirectUri) &&
                    c.Value.Equals(request.LogoutRedirectUri)) != null) : null;

            var app = new ClaimsPrincipal(application ?? new ClaimsIdentity());

            await _loginProvider.LogOutAsync(context.User, app);

            if (app == null)
            {
                return LogoutResult.RedirectToLocalLogoutPage();
            }

            var postLogoutUri = request.LogoutRedirectUri;
            var state = request.Message.State;
            var redirectUri = request.Message.State == null ?
                postLogoutUri :
                QueryHelpers.AddQueryString(postLogoutUri, OpenIdConnectParameterNames.State, state);

            return LogoutResult.Redirect(redirectUri);
        }

        public Task<ClaimsPrincipal> GetUserAsync(string userId)
        {
            return _loginFactory.GetUserAsync(userId);
        }
        public Task<ClaimsPrincipal> GetApplicationAsync(string clientId)
        {
            return _loginFactory.GetApplicationAsync(clientId);
        }
    }
}

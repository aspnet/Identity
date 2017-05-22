﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Extensions;
using Microsoft.AspNetCore.Identity.Service.IntegratedWebClient;
using Microsoft.AspNetCore.Identity.Service.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityWebApplicationSample.Identity.Models;
using System.Security.Claims;

namespace IdentityWebApplicationSample.Identity.Controllers
{
    [Area("IdentityService")]
    public class IdentityServiceController : Controller
    {
        private readonly IOptions<IdentityServiceOptions> _options;
        private readonly ITokenManager _tokenManager;
        private readonly SessionManager<ApplicationUser, IdentityServiceApplication> _sessionManager;
        private readonly IAuthorizationResponseFactory _authorizationResponseFactory;
        private readonly ITokenResponseFactory _tokenResponseFactory;

        public IdentityServiceController(
            IOptions<IdentityServiceOptions> options,
            ITokenManager tokenManager,
            SessionManager<ApplicationUser, IdentityServiceApplication> sessionManager,
            IAuthorizationResponseFactory authorizationResponseFactory,
            ITokenResponseFactory tokenResponseFactory)
        {
            _options = options;
            _tokenManager = tokenManager;
            _sessionManager = sessionManager;
            _authorizationResponseFactory = authorizationResponseFactory;
            _tokenResponseFactory = tokenResponseFactory;
        }

        [HttpGet("tfp/IdentityService/signinsignup/oauth2/v2.0/authorize/")]
        public async Task<IActionResult> Authorize(
            [EnableIntegratedWebClient, ModelBinder(typeof(AuthorizationRequestModelBinder))] AuthorizationRequest authorization)
        {
            if (!authorization.IsValid)
            {
                return this.InvalidAuthorization(authorization.Error);
            }

            var authorizationResult = await _sessionManager.IsAuthorizedAsync(authorization);
            if (authorizationResult.Status == AuthorizationStatus.Forbidden)
            {
                return this.InvalidAuthorization(authorizationResult.Error);
            }

            if (authorizationResult.Status == AuthorizationStatus.LoginRequired)
            {
                return RedirectToLogin(nameof(AccountController.Login), "Account", authorization.Message);
            }

            var context = authorization.CreateTokenGeneratingContext(
                authorizationResult.User,
                authorizationResult.Application);

            context.AmbientClaims.Add(new Claim("policy", "signinsignup"));
            context.AmbientClaims.Add(new Claim("version", "1.0"));
            context.AmbientClaims.Add(new Claim("tenantId", "CDF07358 -BA97-470F-93CD-FC46E1B57F99"));

            await _tokenManager.IssueTokensAsync(context);
            var response = await _authorizationResponseFactory.CreateAuthorizationResponseAsync(context);

            await _sessionManager.StartSessionAsync(authorizationResult.User, authorizationResult.Application);

            return this.ValidAuthorization(response);
        }

        [HttpPost("tfp/IdentityService/signinsignup/oauth2/v2.0/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Token(
            [ModelBinder(typeof(TokenRequestModelBinder))] TokenRequest request)
        {
            if (!request.IsValid)
            {
                return BadRequest(request.Error.Parameters);
            }

            var session = await _sessionManager.CreateSessionAsync(request.UserId, request.ClientId);

            var context = request.CreateTokenGeneratingContext(session.User, session.Application);

            context.AmbientClaims.Add(new Claim("policy", "signinsignup"));
            context.AmbientClaims.Add(new Claim("version", "1.0"));
            context.AmbientClaims.Add(new Claim("tenantId", "CDF07358 -BA97-470F-93CD-FC46E1B57F99"));

            await _tokenManager.IssueTokensAsync(context);
            var response = await _tokenResponseFactory.CreateTokenResponseAsync(context);
            return Ok(response.Parameters);
        }

        [HttpGet("tfp/IdentityService/signinsignup/oauth2/v2.0/logout")]
        public async Task<IActionResult> Logout(
            [EnableIntegratedWebClient, ModelBinder(typeof(LogoutRequestModelBinder))] LogoutRequest request)
        {
            if (!request.IsValid)
            {
                return View("InvalidLogoutRedirect", request.Message);
            }

            var endSessionResult = await _sessionManager.EndSessionAsync(request);
            if (endSessionResult.Status == LogoutStatus.RedirectToLogoutUri)
            {
                return Redirect(endSessionResult.LogoutRedirect);
            }
            else
            {
                return View("LoggedOut", request);
            }
        }

        private IActionResult RedirectToLogin(string action, string controller, OpenIdConnectMessage message)
        {
            var messageCopy = message.Clone();
            messageCopy.Prompt = null;

            var parameters = new
            {
                ReturnUrl = Url.Action("Authorize", "IdentityService", messageCopy.Parameters)
            };

            return RedirectToAction(action, controller, parameters);
        }
    }
}

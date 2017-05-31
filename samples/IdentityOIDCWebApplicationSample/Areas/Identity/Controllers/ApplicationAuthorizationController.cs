using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Applications.Authentication;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.IntegratedWebClient;
using Microsoft.AspNetCore.Identity.Service.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IdentityOIDCWebApplicationSample.Identity.Controllers
{
    [Area("Identity")]
    public class ApplicationAuthorizationController : Controller
    {
        private readonly LoginManager _loginManager;
        private readonly ITokenManager _tokenManager;
        private readonly IAuthorizationResponseFactory _authorizationResponseFactory;
        private readonly ITokenResponseFactory _tokenResponseFactory;

        public ApplicationAuthorizationController(
            ITokenManager tokenManager,
            LoginManager loginManager,
            IAuthorizationResponseFactory authorizationResponseFactory,
            ITokenResponseFactory tokenResponseFactory)
        {
            _loginManager = loginManager;
            _tokenManager = tokenManager;
            _authorizationResponseFactory = authorizationResponseFactory;
            _tokenResponseFactory = tokenResponseFactory;
        }

        [HttpGet("tfp/Identity/signinsignup/oauth2/v2.0/authorize/")]
        public async Task<IActionResult> Authorize(
            [EnableIntegratedWebClient, ModelBinder(typeof(AuthorizationRequestModelBinder))] AuthorizationRequest authorization)
        {
            if (!authorization.IsValid)
            {
                return this.InvalidAuthorization(authorization.Error);
            }

            var authorizationResult = await _loginManager.CanLogIn(authorization);
            if (authorizationResult.Status == LoginStatus.Forbidden)
            {
                return this.InvalidAuthorization(authorizationResult.Error);
            }

            if (authorizationResult.Status == LoginStatus.LoginRequired)
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

            await _loginManager.LogInAsync(authorizationResult.User, authorizationResult.Application);

            return this.ValidAuthorization(response);
        }

        [HttpPost("tfp/Identity/signinsignup/oauth2/v2.0/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Token(
            [ModelBinder(typeof(TokenRequestModelBinder))] TokenRequest request)
        {
            if (!request.IsValid)
            {
                return BadRequest(request.Error.Parameters);
            }

            var user = await _loginManager.GetUserAsync(request.UserId);
            var application = await _loginManager.GetApplicationAsync(request.ClientId);

            var context = request.CreateTokenGeneratingContext(user, application);

            context.AmbientClaims.Add(new Claim("policy", "signinsignup"));
            context.AmbientClaims.Add(new Claim("version", "1.0"));
            context.AmbientClaims.Add(new Claim("tenantId", "CDF07358 -BA97-470F-93CD-FC46E1B57F99"));

            await _tokenManager.IssueTokensAsync(context);
            var response = await _tokenResponseFactory.CreateTokenResponseAsync(context);
            return Ok(response.Parameters);
        }

        [HttpGet("tfp/Identity/signinsignup/oauth2/v2.0/logout")]
        public async Task<IActionResult> Logout(
            [EnableIntegratedWebClient, ModelBinder(typeof(LogoutRequestModelBinder))] LogoutRequest request)
        {
            if (!request.IsValid)
            {
                return View("InvalidLogoutRedirect", request.Message);
            }

            var endSessionResult = await _loginManager.LogOutAsync(request);
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
                ReturnUrl = Url.Action("Authorize", "ApplicationAuthorization", messageCopy.Parameters)
            };

            return RedirectToAction(action, controller, parameters);
        }
    }
}

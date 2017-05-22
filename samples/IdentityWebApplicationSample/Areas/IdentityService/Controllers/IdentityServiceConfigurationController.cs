﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWebApplicationSample.Identity.Controllers
{
    [Area("IdentityService")]
    public class IdentityServiceConfigurationController : Controller
    {
        private readonly IConfigurationManager _configurationProvider;
        private readonly IKeySetMetadataProvider _keySetProvider;

        public IdentityServiceConfigurationController(
            IConfigurationManager configurationProvider,
            IKeySetMetadataProvider keySetProvider)
        {
            _configurationProvider = configurationProvider;
            _keySetProvider = keySetProvider;
        }

        [HttpGet("tfp/IdentityService/signinsignup/v2.0/.well-known/openid-configuration")]
        [Produces("application/json")]
        public async Task<IActionResult> Metadata()
        {
            var configurationContext = new ConfigurationContext
            {
                Id = "IdentityService:signinsignup",
                HttpContext = HttpContext,
                AuthorizationEndpoint = EndpointLink("Authorize", "IdentityService"),
                TokenEndpoint = EndpointLink("Token", "IdentityService"),
                JwksUriEndpoint = EndpointLink("Keys", "IdentityServiceConfiguration"),
                EndSessionEndpoint = EndpointLink("Logout", "IdentityService"),
            };

            return Ok(await _configurationProvider.GetConfigurationAsync(configurationContext));
        }

        [HttpGet("tfp/IdentityService/signinsignup/discovery/v2.0/keys")]
        [Produces("application/json")]
        public async Task<IActionResult> Keys()
        {
            return Ok(await _keySetProvider.GetKeysAsync());
        }

        private string EndpointLink(string action, string controller) =>
            Url.Action(action, controller, null, Request.Scheme, Request.Host.Value);
    }
}

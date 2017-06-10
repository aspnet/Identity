using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Mvc;

namespace Identity.ClientApplications.WebSite.Identity.Controllers
{
    [Area("Identity")]
    public class ConfigurationMetadataController : Controller
    {
        private readonly IConfigurationManager _configurationProvider;
        private readonly IKeySetMetadataProvider _keySetProvider;

        public ConfigurationMetadataController(
            IConfigurationManager configurationProvider,
            IKeySetMetadataProvider keySetProvider)
        {
            _configurationProvider = configurationProvider;
            _keySetProvider = keySetProvider;
        }

        [HttpGet("tfp/Identity/signinsignup/v2.0/.well-known/openid-configuration")]
        [Produces("application/json")]
        public async Task<IActionResult> Metadata()
        {
            var configurationContext = new ConfigurationContext
            {
                Id = "Identity:signinsignup",
                AuthorizationEndpoint = EndpointLink("Authorize", "ApplicationAuthorization"),
                TokenEndpoint = EndpointLink("Token", "ApplicationAuthorization"),
                JwksUriEndpoint = EndpointLink("Keys", "ConfigurationMetadata"),
                EndSessionEndpoint = EndpointLink("Logout", "ApplicationAuthorization"),
            };

            return Ok(await _configurationProvider.GetConfigurationAsync(configurationContext));
        }

        [HttpGet("tfp/Identity/signinsignup/discovery/v2.0/keys")]
        [Produces("application/json")]
        public async Task<IActionResult> Keys()
        {
            return Ok(await _keySetProvider.GetKeysAsync());
        }

        private string EndpointLink(string action, string controller) =>
            Url.Action(action, controller, null, Request.Scheme, Request.Host.Value);
    }
}

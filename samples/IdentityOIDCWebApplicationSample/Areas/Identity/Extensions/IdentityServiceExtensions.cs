﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Identity.Service.Extensions
{
    public static class IdentityApplicationExtensions
    {
        public static IIdentityServiceBuilder AddClientExtensions(this IIdentityServiceBuilder builder)
        {
            builder.Services.Configure<IdentityServiceOptions>(options =>
            {
                options.IdTokenOptions.ContextClaims.AddSingle("tfp", "policy");
                options.IdTokenOptions.ContextClaims.AddSingle("ver", "version");
                options.AccessTokenOptions.ContextClaims.AddSingle("tfp", "policy");
                options.AccessTokenOptions.ContextClaims.AddSingle("ver", "version");
            });

            builder.Services.AddSingleton<IAuthorizationResponseParameterProvider, ClientInfoProvider>();
            builder.Services.AddSingleton<ITokenResponseParameterProvider, ClientInfoProvider>();
            return builder;
        }

        private class ClientInfoProvider : IAuthorizationResponseParameterProvider, ITokenResponseParameterProvider
        {
            public const string ClientInfo = "client_info";

            private readonly IdentityOptions _options;

            public int Order => 100;

            public ClientInfoProvider(IOptions<IdentityOptions> options)
            {
                _options = options.Value;
            }

            public Task AddParameters(TokenGeneratingContext context, AuthorizationResponse response)
            {
                return AddParameters(context, response.Message);
            }

            public Task AddParameters(TokenGeneratingContext context, OpenIdConnectMessage response)
            {
                var clientInfo = CreateClientInfo(context);
                response.Parameters.Add(ClientInfo, clientInfo);
                return Task.CompletedTask;
            }

            public string CreateClientInfo(TokenGeneratingContext context)
            {
                var userId = context.User.Claims.Single(c => string.Equals(c.Type, _options.ClaimsIdentity.UserIdClaimType, StringComparison.Ordinal)).Value;
                var tentantId = context.AmbientClaims.Single(c => string.Equals(c.Type, "tenantId", StringComparison.Ordinal)).Value;

                var json = JsonConvert.SerializeObject(new ClientInfoModel { UserId = userId, TenantId = tentantId });
                return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(json));
            }

            private class ClientInfoModel
            {
                [JsonProperty(PropertyName = "uid")]
                public string UserId { get; set; }
                [JsonProperty(PropertyName = "utid")]
                public string TenantId { get; set; }
            }
        }
    }
}

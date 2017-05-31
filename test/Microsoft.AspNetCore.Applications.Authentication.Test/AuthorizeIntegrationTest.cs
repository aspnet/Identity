// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Applications.Authentication.Internal;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Service.Authorization;
using Microsoft.AspNetCore.Identity.Service.Claims;
using Microsoft.AspNetCore.Identity.Service.Internal;
using Microsoft.AspNetCore.Identity.Service.Signing;
using Microsoft.AspNetCore.Identity.Service.Validation;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class AuthorizeIntegrationTest
    {
        [Fact]
        public async Task Spec_Code_Sample()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = QueryString.FromUriComponent(@"?response_type=code&client_id=s6BhdRkqt3&redirect_uri=https%3A%2F%2Fclient.example.org%2Fcb&scope=openid%20profile%20email&nonce=n-0S6_WzA2Mj&state=af0ifjsldkj");
            var requestParameters = httpContext.Request.Query.ToDictionary(kvp => kvp.Key, kvp => (string[])kvp.Value);

            var requestFactory = CreateRequestFactory();
            var tokenIssuer = GetTokenIssuer();

            var user = CreateUser("user");
            var application = CreateApplication("s6BhdRkqt");
            var responseFactory = CreateAuthorizationResponseFactory();

            var queryExecutor = new QueryResponseGenerator();

            // Act
            var result = await requestFactory.CreateAuthorizationRequestAsync(requestParameters);
            var authorization = result.Message;

            var tokenContext = result.CreateTokenGeneratingContext(user, application);

            await tokenIssuer.IssueTokensAsync(tokenContext);

            var response = await responseFactory.CreateAuthorizationResponseAsync(tokenContext);

            queryExecutor.GenerateResponse(httpContext, response.RedirectUri, response.Message.Parameters);

            // Assert
            Assert.Equal(StatusCodes.Status302Found, httpContext.Response.StatusCode);
            Assert.False(StringValues.IsNullOrEmpty(httpContext.Response.Headers[HeaderNames.Location]));
            var uri = new Uri(httpContext.Response.Headers[HeaderNames.Location]);

            Assert.False(string.IsNullOrEmpty(uri.Query));
            var parameters = QueryHelpers.ParseQuery(uri.Query);

            Assert.Equal(2, parameters.Count);
            var idTokenKvp = Assert.Single(parameters, kvp => kvp.Key == "code");
            var stateKvp = Assert.Single(parameters, kvp => kvp.Key == "state");
            Assert.Equal("af0ifjsldkj", stateKvp.Value);
        }

        [Fact]
        public async Task Spec_IdToken_Sample()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = QueryString.FromUriComponent("?response_type=id_token&client_id=s6BhdRkqt3&redirect_uri=https%3A%2F%2Fclient.example.org%2Fcb&scope=openid%20profile%20email&nonce=n-0S6_WzA2Mj&state=af0ifjsldkj");
            var requestParameters = httpContext.Request.Query.ToDictionary(kvp => kvp.Key, kvp => (string[])kvp.Value);

            var requestFactory = CreateRequestFactory();
            var tokenIssuer = GetTokenIssuer();
            var fragmentExecutor = new FragmentResponseGenerator(UrlEncoder.Default);

            var user = CreateUser("248289761001");
            var application = CreateApplication("s6BhdRkqt");
            var responseFactory = CreateAuthorizationResponseFactory();

            // Act
            var result = await requestFactory.CreateAuthorizationRequestAsync(requestParameters);

            var tokenContext = result.CreateTokenGeneratingContext(user, application);

            await tokenIssuer.IssueTokensAsync(tokenContext);

            var response = await responseFactory.CreateAuthorizationResponseAsync(tokenContext);

            fragmentExecutor.GenerateResponse(httpContext, response.RedirectUri, response.Message.Parameters);

            // Assert
            Assert.Equal(StatusCodes.Status302Found, httpContext.Response.StatusCode);
            Assert.False(StringValues.IsNullOrEmpty(httpContext.Response.Headers[HeaderNames.Location]));
            var uri = new Uri(httpContext.Response.Headers[HeaderNames.Location]);

            Assert.False(string.IsNullOrEmpty(uri.Fragment));
            var parameters = QueryHelpers.ParseQuery(uri.Fragment.Substring(1));

            Assert.Equal(2, parameters.Count);
            var idTokenKvp = Assert.Single(parameters, kvp => kvp.Key == "id_token");
            var stateKvp = Assert.Single(parameters, kvp => kvp.Key == "state");
            Assert.Equal("af0ifjsldkj", stateKvp.Value);
        }

        private static ClaimsPrincipal CreateApplication(string clientId) =>
            new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(TokenClaimTypes.ClientId, clientId) }));

        private static ClaimsPrincipal CreateUser(string userName) =>
            new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userName) }));

        private static TokenManager GetTokenIssuer()
        {
            var options = CreateOptions();
            var claimsManager = GetClaimsManager(options);

            var protector = new EphemeralDataProtectionProvider(new LoggerFactory()).CreateProtector("test");
            var codeSerializer = new TokenDataSerializer<AuthorizationCode>(options, ArrayPool<char>.Shared);
            var codeDataFormat = new DefaultTokenProtector<AuthorizationCode>(codeSerializer, protector);
            var refreshTokenSerializer = new TokenDataSerializer<RefreshToken>(options, ArrayPool<char>.Shared);
            var refreshTokenDataFormat = new DefaultTokenProtector<RefreshToken>(refreshTokenSerializer, protector);

            var timeStampManager = new TimeStampManager();
            var credentialsPolicy = GetCredentialsPolicy(options, timeStampManager);
            var codeIssuer = new AuthorizationCodeIssuer(claimsManager, codeDataFormat, new ProtocolErrorProvider());
            var accessTokenIssuer = new JwtAccessTokenIssuer(claimsManager, credentialsPolicy, new JwtSecurityTokenHandler(), options);
            var idTokenIssuer = new JwtIdTokenIssuer(claimsManager, credentialsPolicy, new JwtSecurityTokenHandler(), options);
            var refreshTokenIssuer = new RefreshTokenIssuer(claimsManager, refreshTokenDataFormat);

            return new TokenManager(
                codeIssuer,
                accessTokenIssuer,
                idTokenIssuer,
                refreshTokenIssuer,
                new ProtocolErrorProvider());
        }

        private static DefaultSigningCredentialsPolicyProvider GetCredentialsPolicy(IOptionsSnapshot<ApplicationTokenOptions> options, TimeStampManager timeStampManager) =>
            new DefaultSigningCredentialsPolicyProvider(
                new List<ISigningCredentialsSource> {
                            new DefaultSigningCredentialsSource(options, timeStampManager)
                },
                timeStampManager);

        private static ITokenClaimsManager GetClaimsManager(
            IOptions<ApplicationTokenOptions> options)
        {
            return new DefaultTokenClaimsManager(
                new List<ITokenClaimsProvider>{
                    new DefaultTokenClaimsProvider(options),
                    new GrantedTokensTokenClaimsProvider(),
                    new NonceTokenClaimsProvider(),
                    new ScopesTokenClaimsProvider(),
                    new TimestampsTokenClaimsProvider(new TimeStampManager(),options),
                    new TokenHashTokenClaimsProvider(new TokenHasher())
                });
        }

        private static IOptionsSnapshot<ApplicationTokenOptions> CreateOptions()
        {
            var identityServiceOptions = new ApplicationTokenOptions();
            var optionsSetup = new IdentityTokensOptionsDefaultSetup();
            optionsSetup.Configure(identityServiceOptions);

            identityServiceOptions.SigningKeys.Add(new SigningCredentials(CryptoUtilities.CreateTestKey(), "RS256"));
            identityServiceOptions.Issuer = "http://server.example.com";
            identityServiceOptions.IdTokenOptions.UserClaims.AddSingle("sub", ClaimTypes.NameIdentifier);

            var mock = new Mock<IOptionsSnapshot<ApplicationTokenOptions>>();
            mock.Setup(m => m.Get(It.IsAny<string>())).Returns(identityServiceOptions);
            mock.Setup(m => m.Value).Returns(identityServiceOptions);

            return mock.Object;
        }

        private IAuthorizationRequestFactory CreateRequestFactory()
        {
            var clientIdValidatorMock = new Mock<IClientIdValidator>();
            clientIdValidatorMock
                .Setup(m => m.ValidateClientIdAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var redirectUriValidatorMock = new Mock<IRedirectUriResolver>();
            redirectUriValidatorMock
                .Setup(m => m.ResolveRedirectUriAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string clientId, string redirectUrl) => RedirectUriResolutionResult.Valid(redirectUrl));

            var scopeValidatorMock = new Mock<IScopeResolver>();
            scopeValidatorMock
                .Setup(m => m.ResolveScopesAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(
                (string clientId, IEnumerable<string> scopes) =>
                    ScopeResolutionResult.Valid(scopes.Select(s => ApplicationScope.CanonicalScopes.TryGetValue(s, out var parsedScope) ? parsedScope : new ApplicationScope(clientId, s))));

            return new AuthorizationRequestFactory(
                clientIdValidatorMock.Object,
                redirectUriValidatorMock.Object,
                scopeValidatorMock.Object,
                Enumerable.Empty<IAuthorizationRequestValidator>(),
                new ProtocolErrorProvider());
        }

        private static DefaultAuthorizationResponseFactory CreateAuthorizationResponseFactory() =>
            new DefaultAuthorizationResponseFactory(new IAuthorizationResponseParameterProvider[] {
                new DefaultAuthorizationResponseParameterProvider(new TimeStampManager())
            });
    }
}

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Xunit;

namespace Microsoft.AspNetCore.Identity.ClientApplications.FunctionalTest
{
    public class Test
    {
        [Fact]
        public async Task CanPerform_AuthorizationCode_Flow()
        {
            // Arrange          
            var clientId = Guid.NewGuid().ToString();
            var resourceId = Guid.NewGuid().ToString();

            var appBuilder = new CredentialsServerBuilder()
                .ConfigureReferenceData(data => data
                    .CreateIntegratedWebClientApplication(clientId)
                    .CreateResourceApplication(resourceId, "ResourceApplication", "read")
                    .CreateUser("testUser", "Pa$$w0rd"))
                .ConfigureInMemoryEntityFrameworkStorage()
                .ConfigureMvcAutomaticSignIn()
                .ConfigureOpenIdConnectClient(options =>
                {
                    options.ClientId = clientId;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.Scope.Add("https://localhost/DFC7191F-FF74-42B9-A292-08FEA80F5B20/v2.0/ResourceApplication/read");
                });

            var client = appBuilder.Build();

            // Act
            var goToAuthorizeResponse = await client.GetAsync("https://localhost/Home/About");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToAuthorizeResponse.StatusCode);

            // Act
            var goToLoginResponse = await client.GetAsync(goToAuthorizeResponse.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToLoginResponse.StatusCode);

            // Act
            var goToAuthorizeWithCookie = await client.GetAsync(goToLoginResponse.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToAuthorizeWithCookie.StatusCode);

            // Act
            var goToSignInOidcCallback = await client.GetAsync(goToAuthorizeWithCookie.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToSignInOidcCallback.StatusCode);

            // Act
            var goToProtectedResource = await client.GetAsync(goToSignInOidcCallback.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToProtectedResource.StatusCode);

            // Act
            var signInOidcCallback = await client.GetAsync(goToProtectedResource.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.OK, signInOidcCallback.StatusCode);

        }

        [Fact]
        public async Task CanPerform_IdToken_Flow()
        {
            // Arrange          
            var clientId = Guid.NewGuid().ToString();
            var resourceId = Guid.NewGuid().ToString();

            var appBuilder = new CredentialsServerBuilder()
                .ConfigureReferenceData(data => data
                    .CreateIntegratedWebClientApplication(clientId)
                    .CreateUser("testUser", "Pa$$w0rd"))
                .ConfigureInMemoryEntityFrameworkStorage()
                .ConfigureMvcAutomaticSignIn()
                .ConfigureOpenIdConnectClient(options =>
                {
                    options.ClientId = clientId;
                });

            var client = appBuilder.Build();

            // Act
            var goToAuthorizeResponse = await client.GetAsync("https://localhost/Home/About");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToAuthorizeResponse.StatusCode);

            // Act
            var goToLoginResponse = await client.GetAsync(goToAuthorizeResponse.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToLoginResponse.StatusCode);

            // Act
            var goToAuthorizeWithCookie = await client.GetAsync(goToLoginResponse.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToAuthorizeWithCookie.StatusCode);

            // Act
            var goToSignInOidcCallback = await client.GetAsync(goToAuthorizeWithCookie.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.OK, goToSignInOidcCallback.StatusCode);
            var document = ResponseAssert.IsHtmlDocument(goToSignInOidcCallback);
            var form = HtmlAssert.HasForm(document, "form");

            // Act
            var goToProtectedResource = await client.SendAsync(form);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, goToProtectedResource.StatusCode);

            // Act
            var protectedResourceResponse = await client.GetAsync(goToProtectedResource.Headers.Location);

            // Assert
            Assert.Equal(HttpStatusCode.OK, protectedResourceResponse.StatusCode);
        }

    }
}

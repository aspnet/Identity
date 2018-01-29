// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class LoginTests
    {
        [Fact]
        public async Task CanLogInWithAPreviouslyRegisteredUser()
        {
            // Arrange
            var builder = WebHostBuilderFactory
                .CreateFromTypesAssemblyEntryPoint<Startup>(new string[] { })
                .UseSolutionRelativeContentRoot(Path.Combine("test", "WebSites", "Identity.DefaultUI.WebSite"));

            var server = new TestServer(builder);
            var client = new HttpClient(new CookieContainerHandler(server.CreateHandler()));
            client.BaseAddress = new Uri("https://localhost");

            // Act & Assert
            var goToIndex = await client.GetAsync("/");
            ResponseAssert.IsOK(goToIndex);
            var index = ResponseAssert.IsHtmlDocument(goToIndex);
            var registerLink = HtmlAssert.HasLinkWithText(index, "Register");

            var goToRegister = await client.GetAsync(registerLink.Href);
            ResponseAssert.IsOK(goToRegister);
            var register = ResponseAssert.IsHtmlDocument(goToRegister);

            var registerForm = HtmlAssert.HasForm(register);
            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var userNameInput = (IHtmlInputElement)registerForm["Input_Email"];
            var passwordInput = (IHtmlInputElement)registerForm["Input_Password"];
            var confirmPasswordInput = (IHtmlInputElement)registerForm["Input_ConfirmPassword"];
            var submitButton = (IHtmlButtonElement)registerForm["register"];
            userNameInput.Value = userName;
            passwordInput.Value = password;
            confirmPasswordInput.Value = password;

            var submit = registerForm.GetSubmission(submitButton);
            var submision = new HttpRequestMessage(new HttpMethod(submit.Method.ToString()), submit.Target)
            {
                Content = new StreamContent(submit.Body)
            };

            foreach (var header in submit.Headers)
            {
                submision.Headers.TryAddWithoutValidation(header.Key, header.Value);
                submision.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var registered = await client.SendAsync(submision);
            var registeredLocation = ResponseAssert.IsRedirect(registered);
            var index2 = await client.GetAsync(registeredLocation);

            ResponseAssert.IsOK(index2);
            var authenticatedIndex = ResponseAssert.IsHtmlDocument(index2);
            HtmlAssert.HasLinkWithText(authenticatedIndex, $"Hello {userName}!");

            // Create a new client to have a complete different context than the used for registration
            var newClient = new HttpClient(new CookieContainerHandler(server.CreateHandler()));
            newClient.BaseAddress = new Uri("https://localhost");

            var goToIndexNewSession = await newClient.GetAsync("/");
            ResponseAssert.IsOK(goToIndexNewSession);
            var frontPage = ResponseAssert.IsHtmlDocument(goToIndexNewSession);
            var loginLink = HtmlAssert.HasLinkWithText(frontPage, "Login");

            var goToLogin = await newClient.GetAsync(loginLink.Href);
            ResponseAssert.IsOK(goToLogin);
            var login = ResponseAssert.IsHtmlDocument(goToLogin);

            var loginForm = HtmlAssert.HasForm(login);

            var loginUserName = (IHtmlInputElement)loginForm["Input_Email"];
            var loginPassword = (IHtmlInputElement)loginForm["Input_Password"];
            var loginSubmit = (IHtmlButtonElement)loginForm["login"];
            loginUserName.Value = userName;
            loginPassword.Value = password;

            var loginSubmission = loginForm.GetSubmission(loginSubmit);
            var loginRequest = new HttpRequestMessage(
                new HttpMethod(
                    loginSubmission.Method.ToString()),
                    loginSubmission.Target)
            {
                Content = new StreamContent(loginSubmission.Body)
            };

            foreach (var header in loginSubmission.Headers)
            {
                loginRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                loginRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var loggedIn = await newClient.SendAsync(loginRequest);
            var loggedInLocation = ResponseAssert.IsRedirect(loggedIn);
            var loggedInIndex = await newClient.GetAsync(loggedInLocation);

            ResponseAssert.IsOK(loggedInIndex);
            var loggedInFrontPage = ResponseAssert.IsHtmlDocument(loggedInIndex);
            HtmlAssert.HasLinkWithText(loggedInFrontPage, $"Hello {userName}!");
        }
    }
}

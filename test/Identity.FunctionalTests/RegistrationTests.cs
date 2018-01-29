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
    public class RegistrationTests
    {
        [Fact]
        public async Task CanRegisterAUser()
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
            goToIndex.EnsureSuccessStatusCode();
            var index = ResponseAssert.IsHtmlDocument(goToIndex);
            var registerLink = HtmlAssert.HasLinkWithText(index, "Register");

            var goToRegister = await client.GetAsync(registerLink.Href);
            goToRegister.EnsureSuccessStatusCode();
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
        }
    }
}

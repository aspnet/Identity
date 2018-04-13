﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Account
{
    public class Login : DefaultUIPage
    {
        private readonly IHtmlFormElement _loginForm;
        private readonly IHtmlAnchorElement _forgotPasswordLink;
        private readonly IHtmlFormElement _externalLoginForm;
        private readonly IHtmlElement _contosoButton;

        public Login(
            HttpClient client,
            IHtmlDocument login,
            DefaultUIContext context)
            : base(client, login, context)
        {
            _loginForm = HtmlAssert.HasForm("#account", login);
            _forgotPasswordLink = HtmlAssert.HasLink("#forgot-password", login);
            if (Context.ContosoLoginEnabled)
            {
                _externalLoginForm = HtmlAssert.HasForm("#external-account", login);
                _contosoButton = HtmlAssert.HasElement("button[value=Contoso]", login);
            }
        }

        public async Task<Contoso.Login> ClickLoginWithContosoLinkAsync()
        {
            var externalFormResponse = await Client.SendAsync(_externalLoginForm, _contosoButton);
            var goToContosoLogin = ResponseAssert.IsRedirect(externalFormResponse);
            var contosoLoginResponse = await Client.GetAsync(goToContosoLogin);

            var contosoLogin = await ResponseAssert.IsHtmlDocumentAsync(contosoLoginResponse);

            return new Contoso.Login(Client, contosoLogin, Context);
        }

        public async Task<ForgotPassword> ClickForgotPasswordLinkAsync()
        {
            var response = await Client.GetAsync(_forgotPasswordLink.Href);
            var forgotPassword = await ResponseAssert.IsHtmlDocumentAsync(response);

            return new ForgotPassword(Client, forgotPassword, Context);
        }

        public async Task<Index> LoginValidUserAsync(string userName, string password)
        {
            var loggedIn = await SendLoginForm(userName, password);

            var loggedInLocation = ResponseAssert.IsRedirect(loggedIn);
            Assert.Equal(Index.Path, loggedInLocation.ToString());
            var indexResponse = await Client.GetAsync(loggedInLocation);
            var index = await ResponseAssert.IsHtmlDocumentAsync(indexResponse);
            return new Index(
                Client,
                index,
                Context.WithAuthenticatedUser());
        }

        public async Task<DefaultUIPage> LockoutUserAsync(string userName, string password)
        {
            var loginAttempt = await SendLoginForm(userName, password);

            var lockedOut = ResponseAssert.IsRedirect(loginAttempt);
            Assert.Equal("/Identity/Account/Lockout", lockedOut.ToString());

            var lockedOutResponse = await Client.GetAsync(lockedOut);
            var lockout = await ResponseAssert.IsHtmlDocumentAsync(lockedOutResponse);
            return new DefaultUIPage(Client, lockout, Context);
        }

        private async Task<HttpResponseMessage> SendLoginForm(string userName, string password)
        {
            return await Client.SendAsync(_loginForm, new Dictionary<string, string>()
            {
                ["Input_Email"] = userName,
                ["Input_Password"] = password
            });
        }

        public async Task<LoginWith2fa> PasswordLoginValidUserWith2FaAsync(string userName, string password)
        {
            var loggedIn = await SendLoginForm(userName, password);

            var loggedInLocation = ResponseAssert.IsRedirect(loggedIn);
            Assert.StartsWith(LoginWith2fa.Path, loggedInLocation.ToString());
            var loginWithTwoFactorResponse = await Client.GetAsync(loggedInLocation);
            var loginWithTwoFactor = await ResponseAssert.IsHtmlDocumentAsync(loginWithTwoFactorResponse);

            return new LoginWith2fa(Client, loginWithTwoFactor, Context);
        }
    }
}

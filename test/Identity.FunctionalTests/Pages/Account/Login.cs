﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Account
{
    public class Login : HtmlPage
    {
        private readonly IHtmlFormElement _loginForm;

        public Login(HttpClient client, IHtmlDocument login, HtmlPageContext context)
            : base(client, login, context)
        {
            _loginForm = HtmlAssert.HasForm(login);
        }

        public async Task<Index> LoginValidUserAsync(string userName, string password)
        {
            var loggedIn = await SendLoginForm(userName, password);

            var loggedInLocation = ResponseAssert.IsRedirect(loggedIn);
            Assert.Equal(Index.Path, loggedInLocation.ToString());
            var indexResponse = await Client.GetAsync(loggedInLocation);
            var index = await ResponseAssert.IsHtmlDocumentAsync(indexResponse);
            return new Index(Client, index, Context, authenticated: true);
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

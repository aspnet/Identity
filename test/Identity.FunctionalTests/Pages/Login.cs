// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    public class Login
    {
        private readonly HttpClient _client;
        private readonly IHtmlDocument _login;
        private readonly IHtmlFormElement _loginForm;

        public Login(HttpClient client, IHtmlDocument login)
        {
            _client = client;
            _login = login;
            _loginForm = HtmlAssert.HasForm(login);
        }

        public async Task<Index> LoginValidUserAsync(string userName, string password)
        {
            var loggedIn = await _client.SendAsync(_loginForm, new Dictionary<string, string>()
            {
                ["Input_Email"] = userName,
                ["Input_Password"] = password
            });

            var loggedInLocation = ResponseAssert.IsRedirect(loggedIn);
            Assert.Equal(Index.Path, loggedInLocation.ToString());
            var indexResponse = await _client.GetAsync(loggedInLocation);
            var index = ResponseAssert.IsHtmlDocument(indexResponse);

            return new Index(_client, index, authenticated: true);
        }
    }
}

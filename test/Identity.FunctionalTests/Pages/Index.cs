// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    public class Index
    {
        private readonly HttpClient _client;
        private readonly IHtmlDocument _index;
        private readonly bool _authenticated;
        private readonly IHtmlAnchorElement _registerLink;
        private readonly IHtmlAnchorElement _loginLink;
        //private readonly IHtmlAnchorElement _logout;
        private readonly IHtmlAnchorElement _manageLink;
        public static readonly string Path = "/";

        public Index(HttpClient client, IHtmlDocument index, bool authenticated)
        {
            _client = client;
            _index = index;
            _authenticated = authenticated;
            if (!_authenticated)
            {
                _registerLink = HtmlAssert.HasLink("#register", _index);
                _loginLink = HtmlAssert.HasLink("#login", _index);
            }
            else
            {
                //_logout = HtmlAssert.HasLink("#logout", _index);
                _manageLink = HtmlAssert.HasLink("#manage", _index);
            }
        }

        public static async Task<Index> CreateAsync(HttpClient client, bool authenticated = false)
        {
            var goToIndex = await client.GetAsync("/");
            var index = ResponseAssert.IsHtmlDocument(goToIndex);

            return new Index(client, index, authenticated);
        }

        public async Task<Register> ClickRegisterLinkAsync()
        {
            Assert.False(_authenticated);

            var goToRegister = await _client.GetAsync(_registerLink.Href);
            var register = ResponseAssert.IsHtmlDocument(goToRegister);

            return new Register(_client, register);
        }

        public async Task<Login> ClickLoginLinkAsync()
        {
            Assert.False(_authenticated);

            var goToLogin = await _client.GetAsync(_loginLink.Href);
            var login = ResponseAssert.IsHtmlDocument(goToLogin);

            return new Login(_client, login);
        }

        internal async Task<Manage> ClickManageLinkAsync()
        {
            Assert.True(_authenticated);

            var goToManage = await _client.GetAsync(_manageLink.Href);
            var manage = ResponseAssert.IsHtmlDocument(goToManage);

            return new Manage(_client, manage);
        }
    }
}

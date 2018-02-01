// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    public class TwoFactor
    {
        private readonly HttpClient _client;
        private readonly IHtmlDocument _twoFactor;
        private readonly bool _twoFactorEnabled;
        private readonly IHtmlAnchorElement _enableAuthenticatorLink;

        public TwoFactor(HttpClient client, IHtmlDocument twoFactor, bool twoFactorEnabled)
        {
            _client = client;
            _twoFactor = twoFactor;
            _twoFactorEnabled = twoFactorEnabled;
            if (!_twoFactorEnabled)
            {
                _enableAuthenticatorLink = HtmlAssert.HasLink("#enable-authenticator", twoFactor);
            }
        }

        internal async Task<EnableAuthenticator> ClickEnableAuthenticatorLinkAsync()
        {
            Assert.False(_twoFactorEnabled);

            var goToEnableAuthenticator = await _client.GetAsync(_enableAuthenticatorLink.Href);
            var enableAuthenticator = ResponseAssert.IsHtmlDocument(goToEnableAuthenticator);

            return new EnableAuthenticator(_client, enableAuthenticator);
        }
    }
}
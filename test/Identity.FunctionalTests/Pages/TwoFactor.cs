// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    public class TwoFactor : HtmlPage
    {
        private readonly bool _twoFactorEnabled;
        private readonly IHtmlAnchorElement _enableAuthenticatorLink;

        public TwoFactor(HttpClient client, IHtmlDocument twoFactor, GlobalContext context, bool twoFactorEnabled)
            : base(client, twoFactor, context)
        {
            _twoFactorEnabled = twoFactorEnabled;
            if (!_twoFactorEnabled)
            {
                _enableAuthenticatorLink = HtmlAssert.HasLink("#enable-authenticator", twoFactor);
            }
        }

        internal async Task<EnableAuthenticator> ClickEnableAuthenticatorLinkAsync()
        {
            Assert.False(_twoFactorEnabled);

            var goToEnableAuthenticator = await Client.GetAsync(_enableAuthenticatorLink.Href);
            var enableAuthenticator = ResponseAssert.IsHtmlDocument(goToEnableAuthenticator);

            return new EnableAuthenticator(Client, enableAuthenticator, Context);
        }
    }
}
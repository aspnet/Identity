// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    public class Manage
    {
        private readonly HttpClient _client;
        private readonly IHtmlDocument _manage;
        private readonly IHtmlAnchorElement _profileLink;
        private readonly IHtmlAnchorElement _changePasswordLink;
        private readonly IHtmlAnchorElement _twoFactorLink;
        private readonly IHtmlAnchorElement _personalDataLink;

        public Manage(HttpClient client, IHtmlDocument manage)
        {
            _client = client;
            _manage = manage;

            _profileLink = HtmlAssert.HasLink("#profile", manage);
            _changePasswordLink = HtmlAssert.HasLink("#change-password", manage);
            _twoFactorLink = HtmlAssert.HasLink("#two-factor", manage);
            _personalDataLink = HtmlAssert.HasLink("#personal-data", manage);
        }

        public async Task<TwoFactor> ClickTwoFactorLinkAsync(bool twoFactorEnabled)
        {
            var goToTwoFactor = await _client.GetAsync(_twoFactorLink.Href);
            var twoFactor = ResponseAssert.IsHtmlDocument(goToTwoFactor);

            return new TwoFactor(_client, twoFactor, twoFactorEnabled);
        }
    }
}

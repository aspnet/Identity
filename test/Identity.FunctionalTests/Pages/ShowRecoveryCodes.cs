// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    internal class ShowRecoveryCodes
    {
        private readonly HttpClient _client;
        private readonly IHtmlDocument _showRecoveryCodes;
        private readonly IEnumerable<IHtmlElement> _recoveryCodeElements;

        public ShowRecoveryCodes(HttpClient client, IHtmlDocument showRecoveryCodes)
        {
            _client = client;
            _showRecoveryCodes = showRecoveryCodes;

            _recoveryCodeElements = HtmlAssert.HasElements(".recovery-code", showRecoveryCodes);
        }

        public IEnumerable<string> Codes => _recoveryCodeElements.Select(rc => rc.TextContent);
    }
}
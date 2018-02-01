// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    internal class EnableAuthenticator
    {
        private readonly HttpClient _client;
        private readonly IHtmlDocument _enableAuthenticator;
        private readonly IHtmlElement _codeElement;
        private readonly IHtmlFormElement _sendCodeForm;

        public EnableAuthenticator(HttpClient client, IHtmlDocument enableAuthenticator)
        {
            _client = client;
            _enableAuthenticator = enableAuthenticator;
            _codeElement = HtmlAssert.HasElement("kbd", enableAuthenticator);
            _sendCodeForm = HtmlAssert.HasForm("#send-code", enableAuthenticator);
        }

        internal async Task<ShowRecoveryCodes> SendValidCodeAsync()
        {
            var code = _codeElement.TextContent.Replace(" ", "");
            var verificationCode = ComputeCode(code);

            var sendCodeResponse = await _client.SendAsync(_sendCodeForm, new Dictionary<string, string>
            {
                ["Input_Code"] = verificationCode
            });

            var goToShowRecoveryCodes = ResponseAssert.IsRedirect(sendCodeResponse);
            var showRecoveryCodesResponse = await _client.GetAsync(goToShowRecoveryCodes);
            var showRecoveryCodes = ResponseAssert.IsHtmlDocument(showRecoveryCodesResponse);

            return new ShowRecoveryCodes(_client, showRecoveryCodes);
        }

        private string ComputeCode(string key)
        {
            var hash = new HMACSHA1(Base32.FromBase32(key));
            var unixTimestamp = Convert.ToInt64(Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
            var timestep = Convert.ToInt64(unixTimestamp / 30);
            return Rfc6238AuthenticationService.ComputeTotp(hash, (ulong)timestep, modifier: null).ToString();
        }
    }
}
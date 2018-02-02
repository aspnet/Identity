// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    public class LoginWithTwoFactor : HtmlPage
    {
        public const string Path = "/Identity/Account/LoginWith2fa";

        private readonly IHtmlFormElement _twoFactorForm;
        private readonly IHtmlAnchorElement _loginWithRecoveryCodeLink;

        public LoginWithTwoFactor(HttpClient client, IHtmlDocument loginWithTwoFactor, GlobalContext context)
            : base(client, loginWithTwoFactor, context)
        {
            _twoFactorForm = HtmlAssert.HasForm(loginWithTwoFactor);
            _loginWithRecoveryCodeLink = HtmlAssert.HasLink("#recovery-code-login", loginWithTwoFactor);
        }

        internal async Task<Index> Send2FACodeAsync(string twoFactorKey)
        {
            var code = EnableAuthenticator.ComputeCode(twoFactorKey);

            var response = await Client.SendAsync(_twoFactorForm, new Dictionary<string, string>
            {
                ["Input_TwoFactorCode"] = code
            });

            var goToIndex = ResponseAssert.IsRedirect(response);
            Assert.Equal(Index.Path, goToIndex.ToString());
            var indexResponse = await Client.GetAsync(goToIndex);
            var index = ResponseAssert.IsHtmlDocument(indexResponse);

            return new Index(Client, index, Context, true);
        }

        internal async Task<LoginWithRecoveryCode> ClickRecoveryCodeLinkAsync()
        {
            var goToLoginWithRecoveryCode = await Client.GetAsync(_loginWithRecoveryCodeLink.Href);
            var loginWithRecoveryCode = ResponseAssert.IsHtmlDocument(goToLoginWithRecoveryCode);

            return new LoginWithRecoveryCode(Client, loginWithRecoveryCode, Context);
        }
    }
}

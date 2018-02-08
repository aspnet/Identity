﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Microsoft.AspNetCore.Identity.FunctionalTests.Account.Manage;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Account
{
    public class LoginWith2fa : HtmlPage
    {
        public const string Path = "/Identity/Account/LoginWith2fa";

        private readonly IHtmlFormElement _twoFactorForm;
        private readonly IHtmlAnchorElement _loginWithRecoveryCodeLink;

        public LoginWith2fa(HttpClient client, IHtmlDocument loginWithTwoFactor, HtmlPageContext context)
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
            var index = await ResponseAssert.IsHtmlDocumentAsync(indexResponse);

            return new Index(Client, index, Context, true);
        }

        internal async Task<LoginWithRecoveryCode> ClickRecoveryCodeLinkAsync()
        {
            var goToLoginWithRecoveryCode = await Client.GetAsync(_loginWithRecoveryCodeLink.Href);
            var loginWithRecoveryCode = await ResponseAssert.IsHtmlDocumentAsync(goToLoginWithRecoveryCode);

            return new LoginWithRecoveryCode(Client, loginWithRecoveryCode, Context);
        }
    }
}

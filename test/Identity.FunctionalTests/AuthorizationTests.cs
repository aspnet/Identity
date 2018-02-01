// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.FunctionalTests.Flows;
using Microsoft.AspNetCore.Identity.FunctionalTests.Infrastructure;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class AuthorizationTests
    {
        public static TheoryData<string> AuthorizedPages =>
            new TheoryData<string>
            {
                "/Identity/Account/Manage/ChangePassword",
                "/Identity/Account/Manage/DeletePersonalData",
                "/Identity/Account/Manage/Disable2fa",
                "/Identity/Account/Manage/DownloadPersonalData",
                "/Identity/Account/Manage/EnableAuthenticator",
                "/Identity/Account/Manage/ExternalLogins",
                "/Identity/Account/Manage/GenerateRecoveryCodes",
                "/Identity/Account/Manage/Index",
                "/Identity/Account/Manage/PersonalData",
                "/Identity/Account/Manage/ResetAuthenticator",
                "/Identity/Account/Manage/SetPassword",
                "/Identity/Account/Manage/ShowRecoveryCodes",
                "/Identity/Account/Manage/TwoFactorAuthentication",
                "/Identity/Account/Logout",
            };

        [Theory]
        [MemberData(nameof(AuthorizedPages))]
        public async Task AnonymousUserCantAccessAuthorizedPages(string url)
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            ResponseAssert.IsRedirect(response);
        }

        // Pages that are not part of a flow and can be accessed directly by
        // typing the url in the browser.
        public static TheoryData<string> RouteableAuthorizedPages =>
            new TheoryData<string>
            {
                        "/Identity/Account/Manage/ChangePassword",
                        "/Identity/Account/Manage/DeletePersonalData",
                        //"/Identity/Account/Manage/Disable2fa",
                        "/Identity/Account/Manage/DownloadPersonalData",
                        "/Identity/Account/Manage/EnableAuthenticator",
                        "/Identity/Account/Manage/ExternalLogins",
                        //"/Identity/Account/Manage/GenerateRecoveryCodes",
                        "/Identity/Account/Manage/Index",
                        "/Identity/Account/Manage/PersonalData",
                        "/Identity/Account/Manage/ResetAuthenticator",
                        //"/Identity/Account/Manage/SetPassword",
                        //"/Identity/Account/Manage/ShowRecoveryCodes",
                        "/Identity/Account/Manage/TwoFactorAuthentication",
                        "/Identity/Account/Logout",
            };

        [Theory]
        [MemberData(nameof(RouteableAuthorizedPages))]
        public async Task AuthenticatedUserCanAccessAuthorizedPages(string url)
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient();
            await AuthenticationFlow.RegisterNewUserAsync(client);
            
            // Act
            var response = await client.GetAsync(url);

            // Assert
            ResponseAssert.IsHtmlDocument(response);
        }

        public static TheoryData<string> UnauthorizedPages =>
            new TheoryData<string> {
                "/Identity/Account/ResetPassword",
                //"/Identity/Account/LoginWithRecoveryCode",
                //"/Identity/Account/LoginWith2fa",
                "/Identity/Account/Login",
                "/Identity/Account/Lockout",
                "/Identity/Account/ForgotPasswordConfirmation",
                "/Identity/Account/ForgotPassword",
                //"/Identity/Account/ExternalLogin",
                //"/Identity/Account/ConfirmEmail",
                "/Identity/Account/AccessDenied",
            };

        [Theory]
        [MemberData(nameof(UnauthorizedPages))]
        public async Task AnonymousUserCanAccessNotAuthorizedPages(string url)
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            ResponseAssert.IsHtmlDocument(response);
        }
    }
}

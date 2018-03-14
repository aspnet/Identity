﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.FunctionalTests.Account;
using Microsoft.AspNetCore.Identity.FunctionalTests.Account.Manage;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class UserStories
    {
        internal static async Task<Index> RegisterNewUserAsync(HttpClient client, string userName = null, string password = null)
        {
            userName = userName ?? $"{Guid.NewGuid()}@example.com";
            password = password ?? $"!Test.Password1$";

            var index = await Index.CreateAsync(client);
            var register = await index.ClickRegisterLinkAsync();

            return await register.SubmitRegisterFormForValidUserAsync(userName, password);
        }

        internal static async Task<Index> LoginExistingUserAsync(HttpClient client, string userName, string password)
        {
            var index = await Index.CreateAsync(client);

            var login = await index.ClickLoginLinkAsync();

            return await login.LoginValidUserAsync(userName, password);
        }

        internal static async Task<Index> RegisterNewUserWithSocialLoginAsync(HttpClient client, string userName, string email)
        {
            var index = await Index.CreateAsync(client, new DefaultUIContext().WithSocialLoginEnabled());

            var login = await index.ClickLoginLinkAsync();

            var contosoLogin = await login.ClickLoginWithContosoLinkAsync();

            var externalLogin = await contosoLogin.SendNewUserNameAsync(userName);

            return await externalLogin.SendEmailAsync(email);
        }

        internal static async Task<Account.Manage.Index> SendEmailConfirmationLinkAsync(Index index)
        {
            var manage = await index.ClickManageLinkAsync();
            return await manage.SendConfirmationEmailAsync();
        }

        internal static async Task<Index> LoginWithSocialLoginAsync(HttpClient client, string userName)
        {
            var index = await Index.CreateAsync(
                client,
                new DefaultUIContext()
                    .WithSocialLoginEnabled()
                    .WithExistingUser());

            var login = await index.ClickLoginLinkAsync();

            var contosoLogin = await login.ClickLoginWithContosoLinkAsync();

            return await contosoLogin.SendExistingUserNameAsync(userName);
        }

        internal static async Task<Index> LoginExistingUser2FaAsync(HttpClient client, string userName, string password, string twoFactorKey)
        {
            var index = await Index.CreateAsync(client);

            var loginWithPassword = await index.ClickLoginLinkAsync();

            var login2Fa = await loginWithPassword.PasswordLoginValidUserWith2FaAsync(userName, password);

            return await login2Fa.Send2FACodeAsync(twoFactorKey);
        }

        internal static async Task<ShowRecoveryCodes> EnableTwoFactorAuthentication(Index index)
        {
            var manage = await index.ClickManageLinkAsync();
            var twoFactor = await manage.ClickTwoFactorLinkAsync();
            var enableAuthenticator = await twoFactor.ClickEnableAuthenticatorLinkAsync();
            return await enableAuthenticator.SendValidCodeAsync();
        }

        internal static async Task<ResetAuthenticator> ResetAuthenticator(Index index)
        {
            var manage = await index.ClickManageLinkAsync();
            var twoFactor = await manage.ClickTwoFactorEnabledLinkAsync();
            var resetAuthenticator = await twoFactor.ClickResetAuthenticatorLinkAsync();
            return await resetAuthenticator.ResetAuthenticatorAsync();
        }

        internal static async Task<Index> LoginExistingUserRecoveryCodeAsync(
            HttpClient client,
            string userName,
            string password,
            string recoveryCode)
        {
            var index = await Index.CreateAsync(client);

            var loginWithPassword = await index.ClickLoginLinkAsync();

            var login2Fa = await loginWithPassword.PasswordLoginValidUserWith2FaAsync(userName, password);

            var loginRecoveryCode = await login2Fa.ClickRecoveryCodeLinkAsync();

            return await loginRecoveryCode.SendRecoveryCodeAsync(recoveryCode);
        }

        internal static async Task<ConfirmEmail> ConfirmEmailAsync(IdentityEmail email, HttpClient client)
        {
            var emailBody = HtmlAssert.IsHtmlFragment(email.Body);
            var linkElement = HtmlAssert.HasElement("a", emailBody);
            var link = Assert.IsAssignableFrom<IHtmlAnchorElement>(linkElement);
            return await ConfirmEmail.Create(link, client, new DefaultUIContext()
                .WithAuthenticatedUser()
                .WithExistingUser()
                .WithConfirmedEmail());
        }

        internal static async Task<ForgotPasswordConfirmation> ForgotPasswordAsync(HttpClient client, string userName)
        {
            var index = await Index.CreateAsync(client);

            var login = await index.ClickLoginLinkAsync();

            var forgotPassword = await login.ClickForgotPasswordLinkAsync();

            return await forgotPassword.SendForgotPasswordAsync(userName);
        }

        internal static async Task<ResetPasswordConfirmation> ResetPasswordAsync(HttpClient client, IdentityEmail resetPasswordEmail, string email, string newPassword)
        {
            var emailBody = HtmlAssert.IsHtmlFragment(resetPasswordEmail.Body);
            var linkElement = HtmlAssert.HasElement("a", emailBody);
            var link = Assert.IsAssignableFrom<IHtmlAnchorElement>(linkElement);

            var resetPassword = await ResetPassword.CreateAsync(link, client, new DefaultUIContext().WithExistingUser());
            return await resetPassword.SendNewPasswordAsync(email, newPassword);
        }

        internal static async Task<ChangePassword> ChangePasswordAsync(Index index, string oldPassword, string newPassword)
        {
            var manage = await index.ClickManageLinkAsync();
            var changePassword = await manage.ClickChangePasswordLinkAsync();

            return await changePassword.ChangePasswordAsync(oldPassword, newPassword);
        }

        internal static async Task<SetPassword> SetPasswordAsync(Index index, string newPassword)
        {
            var manage = await index.ClickManageLinkAsync();
            var setPassword = await manage.ClickChangePasswordLinkExternalLoginAsync();

            return await setPassword.SetPasswordAsync(newPassword);
        }

        internal static async Task<ManageExternalLogin> LinkExternalLoginAsync(Index index, string loginEmail)
        {
            var manage = await index.ClickManageLinkWithExternalLoginAsync();
            var linkLogin = await manage.ClickLinkLoginAsync();

            return await linkLogin.LinkExternalLoginAsync(loginEmail);
        }

        internal static async Task<RemoveExternalLogin> RemoveExternalLoginAsync(ManageExternalLogin manageExternalLogin, string loginEmail)
        {
            // Provide an email to link an external account to
            var removeLogin = await manageExternalLogin.ManageExternalLoginAsync(loginEmail);

            // Remove external login
            return await removeLogin.RemoveLoginAsync("Contoso", "Contoso");
        }

        internal static async Task<Index> DeleteUser(Index index, string password)
        {
            var manage = await index.ClickManageLinkAsync();
            var personalData = await manage.ClickPersonalDataLinkAsync();
            var deleteUser = await personalData.ClickDeleteLinkAsync();
            return await deleteUser.Delete(password);
        }

        internal static async Task<string> DownloadPersonalData(Index index, string userName)
        {
            var manage = await index.ClickManageLinkAsync();
            var personalData = await manage.ClickPersonalDataLinkAsync();
            var download = await personalData.SubmitDownloadForm();
            return await download.Content.ReadAsStringAsync();
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.FunctionalTests.Pages;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Flows
{
    public class AuthenticationFlow
    {
        internal static async Task<Index> RegisterNewUserAsync(HttpClient client, string userName, string password)
        {
            var index = await Index.CreateAsync(client);
            var register = await index.ClickRegisterLinkAsync();

            return await register.CreateValidUser(userName, password);
        }

        internal static async Task<Index> LoginExistingUserAsync(HttpClient client, string userName, string password)
        {
            var index = await Index.CreateAsync(client);

            var login = await index.ClickLoginLinkAsync();

            return await login.LoginValidUserAsync(userName, password);
        }
    }
}

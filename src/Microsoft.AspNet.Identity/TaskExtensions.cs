// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    public static class TaskExtensions
    {
        public static async Task<IdentityResult> WithLoggingAsync<TUser>(this Task<IdentityResult> identityResult, UserManager<TUser> userManager,TUser user,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "") where TUser :class
        {
            return await (await identityResult).WithLoggingAsync(userManager, user, methodName);
        }

        public static async Task<IdentityResult> WithLoggingAsync<TRole>(this Task<IdentityResult> identityResultDelegate, RoleManager<TRole> roleManager, TRole role,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "") where TRole : class
        {
            var result = await identityResultDelegate;

            if (result.Succeeded)
            {
                await roleManager.Logger.WriteInformationAsync(methodName, null, async (callingMethod, exception) =>
                {
                    var baseMessage = Resources.FormatLoggingResultMessageForRole(callingMethod, await roleManager.GetRoleIdAsync(role));

                    return Resources.FormatLogIdentityResultSuccess(baseMessage);
                });
            }
            else
            {
                await roleManager.Logger.WriteWarningAsync(methodName, null, async (callingMethod, exception) =>
                {
                    var baseMessage = Resources.FormatLoggingResultMessageForRole(callingMethod, await roleManager.GetRoleIdAsync(role));

                    return Resources.FormatLogIdentityResultFailure(baseMessage, string.Join(",", result.Errors.Select(x => x.Code).ToList()));
                });
            }

            return result;
        }
    }
}
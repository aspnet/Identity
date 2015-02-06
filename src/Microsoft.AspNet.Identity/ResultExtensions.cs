// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Identity
{
    public static class ResultExtensions
    {
        public static async Task<IdentityResult> WithLoggingAsync<TUser>(this IdentityResult result, UserManager<TUser> userManager, TUser user,
           [System.Runtime.CompilerServices.CallerMemberName] string methodName = "") where TUser : class
        {
            if (result.Succeeded)
            {
                await userManager.Logger.WriteInformationAsync(methodName, null, async (callingMethod, exception) =>
                {
                    var baseMessage = Resources.FormatLoggingResultMessage(callingMethod, await userManager.GetUserIdAsync(user));

                    return Resources.FormatLogIdentityResultSuccess(baseMessage);
                });
            }
            else
            {
                await userManager.Logger.WriteWarningAsync(methodName, null, async (callingMethod, exception) =>
                {
                    var baseMessage = Resources.FormatLoggingResultMessage(callingMethod, await userManager.GetUserIdAsync(user));

                    return Resources.FormatLogIdentityResultFailure(baseMessage, string.Join(",", result.Errors.Select(x => x.Code).ToList()));
                });
            }

            return result;
        }

        public static async Task<bool> WithLoggingAsync<TUser>(this bool result, UserManager<TUser> userManager, TUser user,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "") where TUser : class
        {
            var logLevel = result ? LogLevel.Information : LogLevel.Warning;

            await userManager.Logger.WriteAsync(logLevel, 0, methodName, null, async (callingMethod, exception) =>
            {
                var baseMessage = Resources.FormatLoggingResultMessage(callingMethod, await userManager.GetUserIdAsync(user));

                return string.Format("{0} : {1}", baseMessage, result.ToString());
            });

            return result;
        }

        public static async Task<bool> WithLoggingAsync<TUser>(this bool result, SignInManager<TUser> signInManager, TUser user,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "") where TUser : class
        {
            var logLevel = result ? LogLevel.Information : LogLevel.Warning;

            await signInManager.Logger.WriteAsync(logLevel, 0, methodName, null, async (callingMethod, exception) =>
            {
                return Resources.FormatLoggingSigninResult(Resources.FormatLoggingResultMessage(callingMethod,
            await signInManager.UserManager.GetUserIdAsync(user)), result);
            });

            return result;
        }

        public static async Task<SignInResult> WithLoggingAsync<TUser>(this SignInResult result, SignInManager<TUser> signInManager, TUser user,
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "") where TUser : class
        {
            var status = "";
            if (result.IsLockedOut)
            {
                status = "Lockedout";
            }
            else if (result.IsNotAllowed)
            {
                status = "NotAllowed";
            }
            else if (result.RequiresTwoFactor)
            {
                status = "RequiresTwoFactor";
            }
            else if (result.Succeeded)
            {
                status = "Succeeded";
            }
            else
            {
                status = "Failed";
            }

            await signInManager.Logger.WriteInformationAsync(methodName, null, async (callingMethod, exception) =>
            {
                var baseMessage = Resources.FormatLoggingResultMessage(callingMethod, await signInManager.UserManager.GetUserIdAsync(user));

                return Resources.FormatLoggingSigninResult(baseMessage, status);
            });

            return result;
        }

    }
}
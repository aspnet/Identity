// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Identity
{
    public class IdentityLogger
    {
        public IdentityLogger(ILogger logger)
        {
            Logger = logger;
        }

        protected internal virtual ILogger Logger { get; set; }

        public virtual TResult Log<TResult>(TResult result, Func<TResult, LogLevel> getLevel,
            Func<string> messageAccessor)
        {
            var logLevel = getLevel(result);

            // Check if log level is enabled before creating the message.
            if (Logger.IsEnabled(logLevel))
            {
                Logger.Log(logLevel, 0, messageAccessor(), null, (msg, exp) => (string)msg);
            }

            return result;
        }

        public virtual SignInResult Log(SignInResult result, [CallerMemberName]string methodName = null)
           => Log(result, r => r.GetLogLevel(), () => Resources.FormatLoggingSigninResult(methodName, result));

        public virtual IdentityResult Log(IdentityResult result, [CallerMemberName]string methodName = null)
            => Log(result, r => r.GetLogLevel(), () => Resources.FormatLoggingIdentityResult(methodName, result));

        public virtual bool Log(bool result, [CallerMemberName]string methodName = null)
            => Log<bool>(result, (b) => b ? LogLevel.Verbose : LogLevel.Warning,
                               () => Resources.FormatLoggingIdentityResult(methodName, result));

        public virtual IDisposable BeginScope(string state) => Logger.BeginScope(state);

    }
}
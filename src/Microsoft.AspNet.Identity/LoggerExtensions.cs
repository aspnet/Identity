// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Identity
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static async Task WriteAsync(this ILogger logger, LogLevel logLevel, int eventId, object state, Exception exception,
            Func<object, Exception, Task<string>> formatter)
        {
            if (logger.IsEnabled(logLevel))
            {
                if (formatter != null)
                {
                    var message = await formatter(state, exception);
                    logger.Write(logLevel, eventId, message, null, null);
                }
                else
                {
                    logger.Write(logLevel, eventId, state, exception, null);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static Task WriteInformationAsync(this ILogger logger, object state, Exception exception, Func<object, Exception, Task<string>> formatter)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                return logger.WriteAsync(LogLevel.Information, 0, state, exception, formatter);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static Task WriteWarningAsync(this ILogger logger, object state, Exception exception, Func<object, Exception, Task<string>> formatter)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                return logger.WriteAsync(LogLevel.Warning, 0, state, exception, formatter);
            }

            return Task.FromResult(0);
        }
    }
}
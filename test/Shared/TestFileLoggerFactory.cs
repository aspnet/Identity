// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Identity.Test
{
    public class TestFileLoggerFactory : ILoggerFactory, IDisposable
    {
        private static IList<TestFileLogger> _loggers;

        static TestFileLoggerFactory()
        {
            _loggers = new List<TestFileLogger>();
        }

        public void AddProvider(ILoggerProvider provider)
        {

        }

        public ILogger Create(string name)
        {
            try
            {
                var logger = new TestFileLogger(name);
                _loggers.Add(logger);
                return logger;
            }
            catch (ArgumentException ex)
            {
                // Silently skip if there is already a logger with that key
            }

            return _loggers.Where(x => x.FileName == name).First();
        }

        public void Dispose()
        {
            Parallel.ForEach(_loggers, l =>
            {
                var logger = l as TestFileLogger;
                File.Delete(logger.FileName);
            });
        }
    }
}

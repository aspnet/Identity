// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit.Sdk;
using Xunit.Abstractions;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;

namespace Microsoft.AspNet.Identity.Redis.Test
{
    public class RedisXunitTestFramework : XunitTestFramework
    {
        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new RedisXunitTestExecutor(assemblyName, SourceInformationProvider);
        }
    }

    // TODO - should replace this whole approach with a CollectionFixture when
    // Xunit CollectionFixtures are working correctly.
    public class RedisXunitTestExecutor : XunitTestFrameworkExecutor, IDisposable
    {
        private bool _isDisposed;

        public RedisXunitTestExecutor(
            AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider)
            : base(assemblyName, sourceInformationProvider)
        {
            try
            {
                IdentityRedisTestConfig.GetOrStartServer();
            }
            catch (Exception)
            {
                // do not let exceptions starting server prevent XunitTestFrameworkExecutor from being created
            }
        }

        ~RedisXunitTestExecutor()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                try
                {
                    IdentityRedisTestConfig.StopRedisServer();
                }
                catch (Exception)
                {
                    // do not let exceptions stopping server prevent XunitTestFrameworkExecutor from being disposed
                }

                _isDisposed = true;
            }
        }
    }

    public static class IdentityRedisTestConfig
    {
        internal const string RedisServerExeName = "redis-server.exe";
        internal const string FunctionalTestsRedisServerExeName = "Identity-redis-server";
        internal const string UserProfileRedisNugetPackageServerPath = @".kpm\packages\Redis-64\2.8.9";
        internal const string CIMachineRedisNugetPackageServerPath = @"Redis-64\2.8.9";

        private static volatile Process _redisServerProcess; // null implies if server exists it was not started by this code
        private static readonly object _redisServerProcessLock = new object();
        public static int RedisPort = 611; // override default so that do not interfere with anyone else's server

        public static void GetOrStartServer()
        {
            if (UserHasStartedOwnRedisServer())
            {
                // user claims they have started their own
                return;
            }

            if (AlreadyOwnRunningRedisServer())
            {
                return;
            }

            TryConnectToOrStartServer();
        }

        private static bool AlreadyOwnRunningRedisServer()
        {
            // Does RedisTestConfig already know about a running server?
            return _redisServerProcess != null
                   && !_redisServerProcess.HasExited;
        }

        private static bool TryConnectToOrStartServer()
        {
            if (CanFindExistingRedisServer())
            {
                return true;
            }

            return TryStartRedisServer();
        }

        public static void StopRedisServer()
        {
            if (UserHasStartedOwnRedisServer())
            {
                // user claims they have started their own - they are responsible for stopping it
                return;
            }

            if (CanFindExistingRedisServer())
            {
                lock (_redisServerProcessLock)
                {
                    if (_redisServerProcess != null)
                    {
                        _redisServerProcess.Kill();
                        _redisServerProcess = null;
                    }
                }
            }
        }

        private static bool CanFindExistingRedisServer()
        {
            var process = Process.GetProcessesByName(FunctionalTestsRedisServerExeName).SingleOrDefault();
            if (process == null
                || process.HasExited)
            {
                lock (_redisServerProcessLock)
                {
                    _redisServerProcess = null;
                }
                return false;
            }

            lock (_redisServerProcessLock)
            {
                _redisServerProcess = process;
            }
            return true;
        }

        private static bool TryStartRedisServer()
        {
            var serverPath = GetUserProfileServerPath();
            if (!File.Exists(serverPath))
            {
                serverPath = GetCIMachineServerPath();
                if (!File.Exists(serverPath))
                {
                    throw new Exception("Could not find " + RedisServerExeName +
                                        " at path " + GetUserProfileServerPath() + " nor at " + GetCIMachineServerPath());
                }
            }

            return RunServer(serverPath);
        }

        public static bool UserHasStartedOwnRedisServer()
        {
            // if the user sets this environment variable they are claiming they've started
            // their own Redis Server and are responsible for starting/stopping it
            return (Environment.GetEnvironmentVariable("STARTED_OWN_REDIS_SERVER") != null);
        }

        public static string GetUserProfileServerPath()
        {
            var configFilePath = Environment.GetEnvironmentVariable("USERPROFILE");
            return Path.Combine(configFilePath, UserProfileRedisNugetPackageServerPath, RedisServerExeName);
        }

        public static string GetCIMachineServerPath()
        {
            var configFilePath = Environment.GetEnvironmentVariable("KRE_PACKAGES");
            return Path.Combine(configFilePath, CIMachineRedisNugetPackageServerPath, RedisServerExeName);
        }

        private static bool RunServer(string serverExePath)
        {
            if (_redisServerProcess == null)
            {
                lock (_redisServerProcessLock)
                {
                    // copy the redis-server.exe to a directory under the user's TMP path under a different
                    // name - so we know the difference between a redis-server started by us and a redis-server
                    // which the customer already has running.
                    var tempPath = Path.GetTempPath();
                    var tempRedisServerFullPath =
                        Path.Combine(tempPath, FunctionalTestsRedisServerExeName + ".exe");
                    if (!File.Exists(tempRedisServerFullPath))
                    {
                        File.Copy(serverExePath, tempRedisServerFullPath);
                    }

                    if (_redisServerProcess == null)
                    {
                        var serverArgs = "--port " + RedisPort + " --maxheap 512MB";
                        var processInfo = new ProcessStartInfo
                            {
                                // start the process in users TMP dir (a .dat file will be created but will be removed when the server dies)
                                Arguments = serverArgs,
                                WorkingDirectory = tempPath,
                                CreateNoWindow = true,
                                FileName = tempRedisServerFullPath,
                                RedirectStandardError = true,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                            };
                        try
                        {
                            _redisServerProcess = Process.Start(processInfo);
                            Thread.Sleep(3000); // to give server time to initialize

                            if (_redisServerProcess.HasExited)
                            {
                                throw new Exception("Could not start Redis Server at path "
                                                    + tempRedisServerFullPath + " with Arguments '" + serverArgs + "', working dir = " + tempPath + Environment.NewLine
                                                    + _redisServerProcess.StandardError.ReadToEnd() + Environment.NewLine
                                                    + _redisServerProcess.StandardOutput.ReadToEnd());
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Could not start Redis Server at path "
                                                + tempRedisServerFullPath + " with Arguments '" + serverArgs + "', working dir = " + tempPath, e);
                        }

                        if (_redisServerProcess == null)
                        {
                            throw new Exception("Got null process trying to  start Redis Server at path "
                                                + tempRedisServerFullPath + " with Arguments '" + serverArgs + "', working dir = " + tempPath);
                        }
                    }
                }
            }

            return true;
        }
    }
}
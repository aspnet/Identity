// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore
{
    public static class WebHost
    {
        public static IWebHostBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 58362, lo => lo.UseConnectionLogging());
                    options.Listen(IPAddress.Loopback, 44324, lo =>
                    {
                        X509Certificate2 certificate = null;
                        using (var store = new X509Store(StoreName.My,StoreLocation.CurrentUser))
                        {
                            store.Open(OpenFlags.ReadOnly);
                            certificate = store.Certificates
                                .Find(X509FindType.FindBySubjectDistinguishedName, "CN=localhost", validOnly: false)
                                .OfType<X509Certificate2>()
                                .FirstOrDefault();
                            store.Close();
                        }

                        if (certificate != null)
                        {
                            lo.UseHttps(certificate);
                            return;
                        }

                        using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                        {
                            store.Open(OpenFlags.ReadOnly);
                            certificate = store.Certificates
                                .Find(X509FindType.FindBySubjectDistinguishedName, "CN=localhost", validOnly: false)
                                .OfType<X509Certificate2>()
                                .FirstOrDefault();
                            store.Close();
                        }

                        if (certificate != null)
                        {
                            lo.UseHttps(certificate);
                        }

                        throw new InvalidOperationException("run dotnet msbuild /t:GenerateSSLCertificate");
                    });
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });

            return builder;
        }
    }
}

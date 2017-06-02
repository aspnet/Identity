﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.IntegratedWebClient;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Infrastructure;

namespace Microsoft.AspNetCore
{
    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="IWebHost"/> and <see cref="IWebHostBuilder"/> with pre-configured defaults.
    /// </summary>
    public static class WebHost
    {
        /// <summary>
        /// Initializes and starts a new <see cref="IWebHost"/> with pre-configured defaults.
        /// See <see cref="CreateDefaultBuilder()"/> for details.
        /// </summary>
        /// <param name="app">A delegate that handles requests to the application.</param>
        /// <returns>A started <see cref="IWebHost"/> that hosts the application.</returns>
        public static IWebHost Start(RequestDelegate app) =>
            Start(url: null, app: app);

        /// <summary>
        /// Initializes and starts a new <see cref="IWebHost"/> with pre-configured defaults.
        /// See <see cref="CreateDefaultBuilder()"/> for details.
        /// </summary>
        /// <param name="url">The URL the hosted application will listen on.</param>
        /// <param name="app">A delegate that handles requests to the application.</param>
        /// <returns>A started <see cref="IWebHost"/> that hosts the application.</returns>
        public static IWebHost Start(string url, RequestDelegate app)
        {
            var startupAssemblyName = app.GetMethodInfo().DeclaringType.GetTypeInfo().Assembly.GetName().Name;
            return StartWith(url: url, configureServices: null, app: appBuilder => appBuilder.Run(app), applicationName: startupAssemblyName);
        }

        /// <summary>
        /// Initializes and starts a new <see cref="IWebHost"/> with pre-configured defaults.
        /// See <see cref="CreateDefaultBuilder()"/> for details.
        /// </summary>
        /// <param name="routeBuilder">A delegate that configures the router for handling requests to the application.</param>
        /// <returns>A started <see cref="IWebHost"/> that hosts the application.</returns>
        public static IWebHost Start(Action<IRouteBuilder> routeBuilder) =>
            Start(url: null, routeBuilder: routeBuilder);

        /// <summary>
        /// Initializes and starts a new <see cref="IWebHost"/> with pre-configured defaults.
        /// See <see cref="CreateDefaultBuilder()"/> for details.
        /// </summary>
        /// <param name="url">The URL the hosted application will listen on.</param>
        /// <param name="routeBuilder">A delegate that configures the router for handling requests to the application.</param>
        /// <returns>A started <see cref="IWebHost"/> that hosts the application.</returns>
        public static IWebHost Start(string url, Action<IRouteBuilder> routeBuilder)
        {
            var startupAssemblyName = routeBuilder.GetMethodInfo().DeclaringType.GetTypeInfo().Assembly.GetName().Name;
            return StartWith(url, services => services.AddRouting(), appBuilder => appBuilder.UseRouter(routeBuilder), applicationName: startupAssemblyName);
        }

        /// <summary>
        /// Initializes and starts a new <see cref="IWebHost"/> with pre-configured defaults.
        /// See <see cref="CreateDefaultBuilder()"/> for details.
        /// </summary>
        /// <param name="app">The delegate that configures the <see cref="IApplicationBuilder"/>.</param>
        /// <returns>A started <see cref="IWebHost"/> that hosts the application.</returns>
        public static IWebHost StartWith(Action<IApplicationBuilder> app) =>
            StartWith(url: null, app: app);

        /// <summary>
        /// Initializes and starts a new <see cref="IWebHost"/> with pre-configured defaults.
        /// See <see cref="CreateDefaultBuilder()"/> for details.
        /// </summary>
        /// <param name="url">The URL the hosted application will listen on.</param>
        /// <param name="app">The delegate that configures the <see cref="IApplicationBuilder"/>.</param>
        /// <returns>A started <see cref="IWebHost"/> that hosts the application.</returns>
        public static IWebHost StartWith(string url, Action<IApplicationBuilder> app) =>
            StartWith(url: url, configureServices: null, app: app, applicationName: null);

        private static IWebHost StartWith(string url, Action<IServiceCollection> configureServices, Action<IApplicationBuilder> app, string applicationName)
        {
            var builder = CreateDefaultBuilder();

            if (!string.IsNullOrEmpty(url))
            {
                builder.UseUrls(url);
            }

            if (configureServices != null)
            {
                builder.ConfigureServices(configureServices);
            }

            builder.Configure(app);

            if (!string.IsNullOrEmpty(applicationName))
            {
                builder.UseSetting(WebHostDefaults.ApplicationKey, applicationName);
            }

            var host = builder.Build();

            host.Start();

            return host;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHostBuilder"/> class with pre-configured defaults.
        /// </summary>
        /// <remarks>
        ///   The following defaults are applied to the returned <see cref="WebHostBuilder"/>:
        ///     use Kestrel as the web server,
        ///     set the <see cref="IHostingEnvironment.ContentRootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/>,
        ///     load <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostingEnvironment.EnvironmentName"/>].json',
        ///     load <see cref="IConfiguration"/> from User Secrets when <see cref="IHostingEnvironment.EnvironmentName"/> is 'Development' using the entry assembly,
        ///     load <see cref="IConfiguration"/> from environment variables,
        ///     configures the <see cref="ILoggerFactory"/> to log to the console and debug output,
        ///     enables IIS integration,
        ///     and adds the developer exception page when <see cref="IHostingEnvironment.EnvironmentName"/> is 'Development'
        /// </remarks>
        /// <returns>The initialized <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CreateDefaultBuilder() =>
            CreateDefaultBuilder(args: null);

        /// <summary>
        ///   Initializes a new instance of the <see cref="WebHostBuilder"/> class with pre-configured defaults.
        /// </summary>
        /// <remarks>
        ///   The following defaults are applied to the returned <see cref="WebHostBuilder"/>:
        ///     use Kestrel as the web server,
        ///     set the <see cref="IHostingEnvironment.ContentRootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/>,
        ///     load <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostingEnvironment.EnvironmentName"/>].json',
        ///     load <see cref="IConfiguration"/> from User Secrets when <see cref="IHostingEnvironment.EnvironmentName"/> is 'Development' using the entry assembly,
        ///     load <see cref="IConfiguration"/> from environment variables,
        ///     load <see cref="IConfiguration"/> from supplied command line args,
        ///     configures the <see cref="ILoggerFactory"/> to log to the console and debug output,
        ///     enables IIS integration,
        ///     and adds the developer exception page when <see cref="IHostingEnvironment.EnvironmentName"/> is 'Development'
        /// </remarks>
        /// <param name="args">The command line args.</param>
        /// <returns>The initialized <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
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
                    logging.UseConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureServices(services =>
                {
                    services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
                    services.AddTransient<IConfigureOptions<IdentityServiceOptions>, ConfigureDefaults<IdentityServiceOptions>>();
                    services.AddTransient<IConfigureOptions<OpenIdConnectOptions>, ConfigureDefaults<OpenIdConnectOptions>>();
                    services.AddTransient<IConfigureOptions<IntegratedWebClientOptions>, ConfigureDefaults<IntegratedWebClientOptions>>();
                    services.ConfigureAspNetCoreDefaults();
                });

            return builder;
        }

        public static IServiceCollection ConfigureAspNetCoreDefaults(this IServiceCollection services)
        {
            //services.AddTransient(typeof(IConfigureOptions<>), typeof(ConfigureDefaults<>));
            //services.AddTransient(typeof(IConfigureNamedOptions<>), typeof(ConfigureDefaults<>));
            return services;
        }
    }
}

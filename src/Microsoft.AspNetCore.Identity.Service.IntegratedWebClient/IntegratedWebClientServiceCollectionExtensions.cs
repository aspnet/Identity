// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity.Service.IntegratedWebClient;
using Microsoft.AspNetCore.Identity.Service.IntegratedWebClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IntegratedWebClientServiceCollectionExtensions
    {
        public static IServiceCollection WithIntegratedWebClient(
            this IServiceCollection services,
            Action<IntegratedWebClientOptions> action)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            services.WithIntegratedWebClient();
            services.Configure(action);

            return services;
        }

        public static IServiceCollection WithIntegratedWebClient(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IConfigureOptions<OpenIdConnectOptions>, IntegratedWebClientOpenIdConnectOptionsSetup>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<MvcOptions>, IntegratedWebclientMvcOptionsSetup>());
            return services;
        }
    }
}

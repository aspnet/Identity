// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Certificates.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.Identity.Service.Core
{
    public class IdentityServiceOptionsConfigurationDefaultSetup : ConfigureDefaultOptions<IdentityServiceOptions>
    {
        public const string SectionKey = "Microsoft:AspNetCore:Identity:Service";

        public IdentityServiceOptionsConfigurationDefaultSetup(
            IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment environment)
            : base(options => ConfigureOptions(options, configuration, loggerFactory, environment.EnvironmentName))
        {
        }

        private static void ConfigureOptions(
            IdentityServiceOptions options,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            string environmentName)
        {
            var section = configuration.GetSection(SectionKey);
            var issuer = section.GetValue<string>("Issuer");
            var certificatesSection = section.GetSection("SigningKeys:SigningCertificates");
            if (certificatesSection != null)
            {
                var certificateLoader = new CertificateLoader(configuration.GetSection("Microsoft:AspNetCore:Certificates"), loggerFactory, environmentName);
                var certificates = certificateLoader.Load(certificatesSection);
                foreach (var certificate in certificates)
                {
                    var algorithm = CryptographyHelpers.FindAlgorithm(certificate);
                    options.SigningKeys.Add(new SigningCredentials(new X509SecurityKey(certificate), algorithm));
                }
            }
            if (issuer != null)
            {
                options.Issuer = issuer;
            }
        }
    }
}

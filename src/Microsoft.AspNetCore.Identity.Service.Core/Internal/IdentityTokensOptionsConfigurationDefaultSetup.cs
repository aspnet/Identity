// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Certificates.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public class IdentityTokensOptionsConfigurationDefaultSetup : ConfigureDefaultOptions<ApplicationTokenOptions>
    {
        public const string SectionKey = "Microsoft:AspNetCore:Identity:Service";

        public IdentityTokensOptionsConfigurationDefaultSetup(
            IConfiguration configuration, ILoggerFactory loggerFactory)
            : base(options => ConfigureOptions(options, configuration, loggerFactory))
        {
        }

        private static void ConfigureOptions(
            ApplicationTokenOptions options,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            var section = configuration.GetSection(SectionKey);
            var issuer = section.GetValue<string>("Issuer");
            var certificatesSection = section.GetSection("SigningKeys:SigningCertificates");
            if (certificatesSection != null)
            {
                var certificateLoader = new CertificateLoader(configuration.GetSection("Microsoft:AspNetCore:Certificates"), loggerFactory);
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

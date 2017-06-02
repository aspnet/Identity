// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity.Service.IntegratedWebClient
{
    public class IntegratedWebClientOptions
    {
        public const string TokenRedirectUrn  = "urn:self:aspnet:identity:integrated";

        public string ClientId { get; set; }
        public string AuthorizationEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string EndsSessionEndpoint { get; set; }
    }
}

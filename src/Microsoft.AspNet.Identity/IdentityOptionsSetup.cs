// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Identity
{
    public class IdentityOptionsSetup : IOptionsSetup<IdentityOptions>
    {
        private readonly IConfiguration _config;

        public IdentityOptionsSetup(IConfiguration config)
        {
            _config = config;
        }

        public int Order { get; set; }

        public void Setup(IdentityOptions options)
        {
            if (_config != null)
            {
                OptionsServices.ReadProperties(options.ClaimType, _config.GetSubKey("identity:claimtype"));
                OptionsServices.ReadProperties(options.User, _config.GetSubKey("identity:user"));
                OptionsServices.ReadProperties(options.Password, _config.GetSubKey("identity:password"));
                OptionsServices.ReadProperties(options.Lockout, _config.GetSubKey("identity:lockout"));
            }
        }
    }
}
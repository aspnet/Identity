// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Identity
{
    public class IdentityOptionsSetup : ConfigOptionsSetup<IdentityOptions>
    {
        private readonly IConfiguration _config;

        public IdentityOptionsSetup(IConfiguration config) : base(config)
        {
            _config = config;
        }

        public override void Setup(IdentityOptions options)
        {
            if (_config != null)
            {
                OptionsServices.ReadProperties(options, _config.GetSubKey("identity"));
            }
        }
    }
}
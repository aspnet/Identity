// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
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
                ReadProperties(options.ClaimType, _config.GetSubKey("identity:claimtype"));
                ReadProperties(options.User, _config.GetSubKey("identity:user"));
                ReadProperties(options.Password, _config.GetSubKey("identity:password"));
                ReadProperties(options.Lockout, _config.GetSubKey("identity:lockout"));
            }
        }

        // TODO: Move this somewhere common (Config?) or at least the config.Get conversion
        public static void ReadProperties(object obj, IConfiguration config)
        {
            if (obj == null || config == null)
            {
                return;
            }
            var props = obj.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                {
                    continue;
                }
                var configValue = config.Get(prop.Name);
                if (configValue == null)
                {
                    continue;
                }

                try
                {
// No convert on portable
#if NET45 || K10
                    prop.SetValue(obj, Convert.ChangeType(configValue, prop.PropertyType));
#endif
                }
                catch
                {
                    // TODO: what do we do about errors?
                }
            }
        }
    }
}
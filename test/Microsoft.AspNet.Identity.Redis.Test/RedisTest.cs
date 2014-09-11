// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity.Test;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Identity.Redis.Test
{
    public class SimpleFixture
    {
        public DbContext CreateContext()
        {
            var options = new DbContextOptions()
                .UseModel(CreateModel())
                .UseRedis("127.0.0.1", 80);

            return new DbContext(options);
        }

        public IModel CreateModel()
        {
            var model = new Model();
            var builder = new BasicModelBuilder(model);
            builder.Entity<SimplePoco>(b =>
                {
                    b.Key(cust => cust.PocoKey);
                    b.Property(cust => cust.PocoKey);
                    b.Property(cust => cust.Name);
                });

            return model;
        }
    }
}

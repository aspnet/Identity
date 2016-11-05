// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public static class DbUtil
    {

        public static IServiceCollection ConfigureDbServices(string connectionString, IServiceCollection services = null) 
        {
            if (services == null)
            {
                services = new ServiceCollection();
            }
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			LinqToDB.Data.DataConnection.AddConfiguration("*", connectionString, new SqlServerDataProvider("*", SqlServerVersion.v2008));

            return services;
        }

        //public static TContext Create<TContext>(string connectionString) where TContext : DbContext
        //{
        //    var serviceProvider = ConfigureDbServices<TContext>(connectionString).BuildServiceProvider();
        //    return serviceProvider.GetRequiredService<TContext>();
        //}
    }
}
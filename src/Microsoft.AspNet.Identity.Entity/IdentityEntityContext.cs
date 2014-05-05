// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using System;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.Data.Entity;
using Microsoft.Data.SqlServer;
using Microsoft.Data.Entity.Metadata;

namespace Microsoft.AspNet.Identity.Entity
{
    public class IdentityEntityContext :
        IdentityEntityContext<User>
    {
        public IdentityEntityContext() { }
        public IdentityEntityContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }

    public class IdentityEntityContext<TUser> : DbContext
        where TUser : User
    {

        public DbSet<TUser> Users { get; set; }
        //public DbSet<TRole> Roles { get; set; }

        public IdentityEntityContext(IServiceProvider serviceProvider)
        : base(serviceProvider) { }

        public IdentityEntityContext() { }

        protected override void OnConfiguring(EntityConfigurationBuilder builder)
        {
            // TODO: pull connection string from config
            builder.SqlServerConnectionString(@"Server=(localdb)\v11.0;Database=SimpleIdentity3;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TUser>()
                .Key(u => u.Id)
                .Properties(ps => ps.Property(u => u.UserName))
                .ToTable("AspNetUsers");
        }
    }
}
using Microsoft.AspNet.Testing;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.InMemory;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Entity.Test
{
    public static class TestIdentityFactory
    {
        public static DbContext CreateContext()
        {
            var serviceProvider = new ServiceCollection()
                        .AddEntityFramework(s => s.AddInMemoryStore())
                        .BuildServiceProvider();

            var db = new IdentityContext(serviceProvider);
            //            var sql = db.Configuration.DataStore as SqlServerDataStore;
            //            if (sql != null)
            //            {
            //#if NET45
            //                var builder = new DbConnectionStringBuilder {ConnectionString = sql.ConnectionString};
            //                var targetDatabase = builder["Database"].ToString();

            //                // Connect to master, check if database exists, and create if not
            //                builder.Add("Database", "master");
            //                using (var masterConnection = new SqlConnection(builder.ConnectionString))
            //                {
            //                    masterConnection.Open();

            //                    var masterCommand = masterConnection.CreateCommand();
            //                    masterCommand.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE [name]=N'" + targetDatabase +
            //                                                "'";
            //                    if ((int?) masterCommand.ExecuteScalar() < 1)
            //                    {
            //                        masterCommand.CommandText = "CREATE DATABASE [" + targetDatabase + "]";
            //                        masterCommand.ExecuteNonQuery();

            //                        using (var conn = new SqlConnection(sql.ConnectionString))
            //                        {
            //                            conn.Open();
            //                            var command = conn.CreateCommand();
            //                            command.CommandText = @"
            //CREATE TABLE [dbo].[AspNetUsers] (
            //[Id]                   NVARCHAR (128) NOT NULL,
            //[Email]                NVARCHAR (256) NULL,
            //[EmailConfirmed]       BIT            NOT NULL,
            //[PasswordHash]         NVARCHAR (MAX) NULL,
            //[SecurityStamp]        NVARCHAR (MAX) NULL,
            //[PhoneNumber]          NVARCHAR (MAX) NULL,
            //[PhoneNumberConfirmed] BIT            NOT NULL,
            //[TwoFactorEnabled]     BIT            NOT NULL,
            //[LockoutEndDateUtc]    DATETIME       NULL,
            //[LockoutEnabled]       BIT            NOT NULL,
            //[AccessFailedCount]    INT            NOT NULL,
            //[UserName]             NVARCHAR (256) NOT NULL
            //) ";
            //                            //CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
            //                            command.ExecuteNonQuery();
            //                        }
            //                    }
            //                }
            //#else
            //                throw new NotSupportedException("SQL Server is not yet supported when running against K10.");
            //#endif
            //}


            // TODO: CreateAsync DB?
            return db;
        }

        public class TestSetup : IOptionsSetup<IdentityOptions>
        {
            private readonly IdentityOptions _options;

            public TestSetup(IdentityOptions options)
            {
                _options = options;
            }

            public int Order { get { return 0; } }
            public void Setup(IdentityOptions options)
            {
                options.Copy(_options);
            }
        }


        public static UserManager<EntityUser> CreateManager(DbContext context)
        {
            var services = new ServiceCollection();
            services.AddTransient<IUserValidator<EntityUser>, UserValidator<EntityUser>>();
            services.AddTransient<IPasswordValidator, PasswordValidator>();
            services.AddInstance<IUserStore<EntityUser>>(new UserStore<EntityUser>(context));
            services.AddSingleton<UserManager<EntityUser>, UserManager<EntityUser>>();
            var options = new IdentityOptions
            {
                PasswordsRequireDigit = false,
                PasswordsRequireLowercase = false,
                PasswordsRequireNonLetterOrDigit = false,
                PasswordsRequireUppercase = false
            };
            var optionsAccessor = new OptionsAccessor<IdentityOptions>(new[] { new TestSetup(options) });
            //services.AddInstance<IOptionsAccessor<IdentityOptions>>(new OptionsAccessor<IdentityOptions>(new[] { new TestSetup(options) }));
            //return services.BuildServiceProvider().GetService<UserManager<EntityUser>>();
            return new UserManager<EntityUser>(services.BuildServiceProvider(), new UserStore<EntityUser>(context), optionsAccessor);
        }

        public static UserManager<EntityUser> CreateManager()
        {
            return CreateManager(CreateContext());
        }

        public static RoleManager<EntityRole> CreateRoleManager(DbContext context)
        {
            var services = new ServiceCollection();
            services.AddTransient<IRoleValidator<EntityRole>, RoleValidator<EntityRole>>();
            services.AddInstance<IRoleStore<EntityRole>>(new RoleStore<EntityRole, string>(context));
//            return services.BuildServiceProvider().GetService<RoleManager<EntityRole>>();
            return new RoleManager<EntityRole>(services.BuildServiceProvider(), new RoleStore<EntityRole, string>(context));
        }

        public static RoleManager<EntityRole> CreateRoleManager()
        {
            return CreateRoleManager(CreateContext());
        }
    }
}

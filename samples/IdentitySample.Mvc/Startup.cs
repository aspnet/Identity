using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentitySample.Models;
using IdentitySample.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Identity;

namespace IdentitySample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set connection configuration
	        DataConnection
		        .AddConfiguration(
			        "Default",
			        Configuration["Data:DefaultConnection:ConnectionString"],
			        new SqlServerDataProvider("Default", SqlServerVersion.v2012));

	        DataConnection.DefaultConfiguration = "Default";

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Cookies.ApplicationCookie.AuthenticationScheme = "ApplicationCookie";
                options.Cookies.ApplicationCookie.CookieName = "Interop";
                options.Cookies.ApplicationCookie.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo("C:\\Github\\Identity\\artifacts"));
            })
                .AddLinqToDBStores(new DefaultConnectionFactory<DataContext, ApplicationDataConnection>())
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

	        var connectionString = new SqlConnectionStringBuilder(Configuration["Data:DefaultConnection:ConnectionString"])
	        {
		        InitialCatalog = "master"
	        }.ConnectionString;

	        using (var db = new DataConnection(SqlServerTools.GetDataProvider(), connectionString))
	        {
		        try
		        {
			        var sql = "create database [" +
			                  new SqlConnectionStringBuilder(Configuration["Data:DefaultConnection:ConnectionString"])
				                  .InitialCatalog + "]";
			        db.Execute(sql);
		        }
		        catch
		        {
			        //
		        }

	        }

			// Try to create tables
			using (var db = new ApplicationDataConnection())
			{
				TryCreateTable<ApplicationUser>(db);
				TryCreateTable<IdentityRole>(db);
				TryCreateTable<IdentityUserClaim<string>>(db);
				TryCreateTable<IdentityRoleClaim<string>>(db);
				TryCreateTable<IdentityUserLogin<string>>(db);
				TryCreateTable<IdentityUserRole<string>>(db);
				TryCreateTable<IdentityUserToken<string>>(db);

			}

			app.UseStaticFiles();

            app.UseIdentity();
            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

	    private void TryCreateTable<T>(ApplicationDataConnection db)
			where T : class 
	    {
		    try
		    {
			    db.CreateTable<T>();
		    }
		    catch
		    {
			    //
		    }
	    }
    }
}


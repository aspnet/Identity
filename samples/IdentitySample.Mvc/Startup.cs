using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Authentication;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.AspNet.Security.Facebook;
using Microsoft.AspNet.Security.Google;
using Microsoft.AspNet.Security.Twitter;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using IdentitySample.Models;
using System;
using Microsoft.AspNet.Security;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;

namespace IdentitySamples
{
    public static class UseExt {

        public class InstanceOptionsAccessor<TOptions>(TOptions options) : IOptionsAccessor<TOptions> where TOptions : class, new()
        {
            public TOptions Options { get; } = options;
        }

        public static void UseOptions<TOptions>(this IServiceCollection services, TOptions options) where TOptions : class, new()
        {
            services.AddInstance<IOptionsAccessor<TOptions>>(new InstanceOptionsAccessor<TOptions>(options));
        }
    }

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            /* Adding IConfiguration as a service in the IoC to avoid instantiating Configuration again.
                 * Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
                 * then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            */
            var configuration = new Configuration();
            configuration.AddJsonFile("LocalConfig.json");
            configuration.AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.

            var identityOptions = new IdentityCookieOptions<ApplicationUser>();
            identityOptions.Password.RequireDigit = false;
            identityOptions.Password.RequireLowercase = false;
            identityOptions.Password.RequireUppercase = false;
            identityOptions.Password.RequireNonLetterOrDigit = false;

            app.UseServices(services =>
            {
                services.UseOptions(identityOptions);

                // Add EF services to the services container
                services.AddEntityFramework()
                        .AddSqlServer();

                // Configure DbContext           
                services.SetupOptions<IdentityDbContextOptions>(options =>
                {
                    options.DefaultAdminUserName = configuration.Get("DefaultAdminUsername");
                    options.DefaultAdminPassword = configuration.Get("DefaultAdminPassword");
                    options.UseSqlServer(configuration.Get("Data:IdentityConnection:ConnectionString"));
                });

                // Add Identity services to the services container
                services.AddIdentitySqlServer<ApplicationDbContext, ApplicationUser>()
                        .AddAuthentication();

                // Add MVC services to the services container
                services.AddMvc();
            });

            /* Error page middleware displays a nice formatted HTML page for any unhandled exceptions in the request pipeline.
             * Note: ErrorPageOptions.ShowAll to be used only at development time. Not recommended for production.
             */
            app.UseErrorPage(ErrorPageOptions.ShowAll);

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Setup identity cookie middleware
            // Add cookie-based authentication to the request pipeline
            app.UseIdentity(identityOptions);

            app.UseGoogleAuthentication(new GoogleAuthenticationOptions
            {
                ClientId = "514485782433-fr3ml6sq0imvhi8a7qir0nb46oumtgn9.apps.googleusercontent.com",
                ClientSecret = "V2nDD9SkFbvLTqAUBWBBxYAL"
            });

            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AppId = "901611409868059",
                AppSecret = "4aa3c530297b1dcebc8860334b39668b"
            });

            app.UseTwitterAuthentication(new TwitterAuthenticationOptions
            {
                ConsumerKey = "BSdJJ0CrDuvEhpkchnukXZBUv",
                ConsumerSecret = "xKUNuKhsRdHD03eLn67xhPAyE1wFFEndFo1X2UJaK2m1jdAxf4"
            });
            
            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });

            //Populates the Admin user and role
            SampleData.InitializeIdentityDatabaseAsync(app.ApplicationServices).Wait();
        }
    }
}
using IdentitySample.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.RequestContainer;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.AspNet.Security.Facebook;
using Microsoft.AspNet.Security.Google;
using Microsoft.AspNet.Security.Twitter;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;
using System;

namespace IdentitySamples
{
    public partial class Startup
    {
        public Startup()
        {
            /* 
            * Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            * then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            */
            Configuration = new Configuration()
                .AddJsonFile("LocalConfig.json")
                .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.
        }

        public IConfiguration Configuration { get; private set; }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOptions();

            app.UseEntityFramework()
               .AddSqlServer();

            app.Use<ApplicationDbContext>(); // Replaced services.AddScoped<>

            app.Configure<IdentityDbContextOptions>(options =>
            {
                options.DefaultAdminUserName = Configuration.Get("DefaultAdminUsername");
                options.DefaultAdminPassword = Configuration.Get("DefaultAdminPassword");
                options.UseSqlServer(Configuration.Get("Data:IdentityConnection:ConnectionString"));
            });

            app.UseErrorPage(ErrorPageOptions.ShowAll)
               .UseStaticFile()
                //.UseSignalR()
                .UseDefaultIdentity<ApplicationDbContext, ApplicationUser, IdentityRole>(Configuration, options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonLetterOrDigit = false;
                    options.SecurityStampValidationInterval = TimeSpan.FromMinutes(20);
                })
                .UseFacebookAuthentication(options =>
                {
                    options.AppId = "901611409868059";
                    options.AppSecret = "4aa3c530297b1dcebc8860334b39668b";
                })
                .UseGoogleAuthentication(options =>
                {
                    options.ClientId = "514485782433-fr3ml6sq0imvhi8a7qir0nb46oumtgn9.apps.googleusercontent.com";
                    options.ClientSecret = "V2nDD9SkFbvLTqAUBWBBxYAL";
                })
                .UseTwitterAuthentication(options =>
                {
                    options.ConsumerKey = "BSdJJ0CrDuvEhpkchnukXZBUv";
                    options.ConsumerSecret = "xKUNuKhsRdHD03eLn67xhPAyE1wFFEndFo1X2UJaK2m1jdAxf4";
                })
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}/{id?}",
                        defaults: new { controller = "Home", action = "Index" });
                });

            //Populates the Admin user and role
            //SampleData.InitializeIdentityDatabaseAsync(app.ApplicationServices).Wait();
        }
    }

    public static class UseExt
    {
        public static IServiceCollection UseOptions(this IApplicationBuilder builder)
        {
            return builder.UseServices.Add(OptionsServices.GetDefaultServices());
        }

        public static IServiceCollection Use<TService>(this IApplicationBuilder builder)
        {
            return builder.UseServices.AddScoped<TService>();
        }

        public static EntityServicesBuilder UseEntityFramework(this IApplicationBuilder builder)
        {
            return builder.UseServices.AddEntityFramework();
        }

        public static IApplicationBuilder UseDefaultIdentity<TContext, TUser, TRole>(this IApplicationBuilder builder, IConfiguration config = null, Action<IdentityOptions> configure = null)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TContext : DbContext
        {
            builder.UseServices.AddDefaultIdentity<TContext, TUser, TRole>(config);
            return builder.UseIdentity(configure);
        }

        public static IApplicationBuilder UseIdentity(this IApplicationBuilder builder, Action<IdentityOptions> configure = null)
        {
            if (configure != null)
            {
                builder.UseServices.SetupOptions(configure);
            }
            builder.UseCookieAuthentication(IdentityOptions.ExternalCookieAuthenticationType); // This should take authType and look up options
            builder.UseCookieAuthentication(IdentityOptions.TwoFactorRememberMeCookieAuthenticationType); // This should take authType and look up options
            builder.UseCookieAuthentication(IdentityOptions.TwoFactorUserIdCookieAuthenticationType); // This should take authType and look up options
            builder.UseCookieAuthentication(IdentityOptions.ApplicationCookieAuthenticationType); // This should take authType and look up options
            return builder;
        }

        public static IApplicationBuilder UseMvc(this IApplicationBuilder builder, Action<IRouteBuilder> configureRoutes = null)
        {
            builder.UseServices.AddMvc();

            // TODO: this should switch to pulling some kind of RouterOptions instead
            var sp = builder.UseServices.BuildServiceProvider(builder.ApplicationServices);
            var routes = new RouteBuilder
            {
                DefaultHandler = new MvcRouteHandler(),
                ServiceProvider = sp
            };

            routes.Routes.Add(AttributeRouting.CreateAttributeMegaRoute(
                routes.DefaultHandler,
                sp));

            configureRoutes(routes);

            return builder.UseMiddleware<RouterMiddleware>(routes.Build());
        }

        public static IApplicationBuilder UseStaticFile(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StaticFileMiddleware>(new StaticFileOptions());
        }

        public static IApplicationBuilder UseErrorPage(this IApplicationBuilder builder, ErrorPageOptions option)
        {
            return builder.UseMiddleware<ErrorPageMiddleware>(option, true);
        }
    }
}
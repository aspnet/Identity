using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.StaticFiles;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.AspNet.Security.Facebook;
using Microsoft.AspNet.Security.Google;
using Microsoft.AspNet.Security.Twitter;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;
using IdentitySample.Models;
using System.Collections.Generic;
using Microsoft.AspNet.Security;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.RequestContainer;

namespace IdentitySamples
{
    public static class UseExt {
        public static IApplicationBuilder Configure(this IApplicationBuilder builder,
            Action<ServiceCollection, PipelineBuilder> configure, IConfiguration config = null)
        {
            // This is what Use Services is doing
            var serviceCollection = new ServiceCollection();
            var pipelineBuilder = new PipelineBuilder(builder, serviceCollection, config);

            serviceCollection.Add(OptionsServices.GetDefaultServices());

            configure(serviceCollection, pipelineBuilder);
            builder.ApplicationServices = serviceCollection.BuildServiceProvider(builder.ApplicationServices);

            return builder;
        }

        public static IApplicationBuilder BuildPipeline(this IApplicationBuilder builder)
        {
            // Services always go first
            builder.UseMiddleware(typeof(ContainerMiddleware));
            foreach (var middlewareOptions in builder.Pipeline)
            {
                builder.UseMiddleware(middlewareOptions.Type, middlewareOptions.Args);
            }
            return builder;
        }

        public static PipelineBuilder ConfigurePipeline(this IApplicationBuilder app, IServiceCollection services, IConfiguration config = null, Action<PipelineBuilder> configure = null)
        {
            var pipeline = new PipelineBuilder(app, services, config);
            if (configure != null)
            {
                configure(pipeline);
            }
            return pipeline;
        }

    }

    public class PipelineBuilder
    {
        public PipelineBuilder(IApplicationBuilder app, IServiceCollection services, IConfiguration config)
        {
            App = app;
            Services = services;
            Configuration = config;
        }

        public IApplicationBuilder App { get; private set; }
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public PipelineBuilder UseMiddleware<TMiddleware>(params object[] args) where TMiddleware : class
        {
            App.Pipeline.Add(new MiddlewareOptions
            {
                Type = typeof(TMiddleware),
                Args = args
            });
            return this;
        }

        public PipelineBuilder UseDefaultIdentity<TContext, TUser, TRole>(Action<IdentityOptions> configure = null)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TContext : DbContext
        {
            Services.AddDefaultIdentity<TContext, TUser, TRole>(Configuration);
            return UseIdentity(configure);
        }


        public PipelineBuilder UseIdentity(Action<IdentityOptions> configure = null)
        {
            if (configure != null)
            {
                Services.SetupOptions(configure);
            }
            UseMiddleware<CookieAuthenticationMiddleware>(IdentityOptions.ExternalCookieAuthenticationType); // This should take authType and look up options
            UseMiddleware<CookieAuthenticationMiddleware>(IdentityOptions.TwoFactorRememberMeCookieAuthenticationType); // This should take authType and look up options
            UseMiddleware<CookieAuthenticationMiddleware>(IdentityOptions.TwoFactorUserIdCookieAuthenticationType); // This should take authType and look up options
            UseMiddleware<CookieAuthenticationMiddleware>(IdentityOptions.ApplicationCookieAuthenticationType); // This should take authType and look up options
            return this;
        }

        public PipelineBuilder UseFacebookAuthentication(Action<FacebookAuthenticationOptions> configure = null)
        {
            if (configure != null)
            {
                Services.SetupOptions(configure);
            }
            return UseMiddleware<FacebookAuthenticationMiddleware>("");
        }

        public PipelineBuilder UseGoogleAuthentication(Action<GoogleAuthenticationOptions> configure = null)
        {
            if (configure != null)
            {
                Services.SetupOptions(configure);
            }
            return UseMiddleware<GoogleAuthenticationMiddleware>("");
        }

        public PipelineBuilder UseTwitterAuthentication(Action<TwitterAuthenticationOptions> configure = null)
        {
            if (configure != null)
            {
                Services.SetupOptions(configure);
            }
            return UseMiddleware<TwitterAuthenticationMiddleware>("");
        }

        public PipelineBuilder UseMvc(Action<IRouteBuilder> configureRoutes = null)
        {
            Services.AddMvc();

            // REVIEW: this uses services :(
            //var sp = Services.BuildServiceProvider();
            //var routes = new RouteBuilder
            //{
            //    DefaultHandler = new MvcRouteHandler(),
            //    ServiceProvider = sp
            //};

            //routes.Routes.Add(AttributeRouting.CreateAttributeMegaRoute(
            //    routes.DefaultHandler,
            //    sp));

            //configureRoutes(routes);

            //return UseMiddleware<RouterMiddleware>(routes);
            return this;
        }

        public PipelineBuilder UseStaticFile()
        {
            return UseMiddleware<StaticFileMiddleware>(new StaticFileOptions());
        }

        public PipelineBuilder UseErrorPage(ErrorPageOptions option)
        {
            return UseMiddleware<ErrorPageMiddleware>(option, true);
        }
    }

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
            app.Configure((services, pipeline) =>
            {
                // Add EF services to the services container
                services.AddEntityFramework()
                        .AddSqlServer();

                // Configure DbContext           
                services.SetupOptions<IdentityDbContextOptions>(options =>
                {
                    options.DefaultAdminUserName = Configuration.Get("DefaultAdminUsername");
                    options.DefaultAdminPassword = Configuration.Get("DefaultAdminPassword");
                    options.UseSqlServer(Configuration.Get("Data:IdentityConnection:ConnectionString"));
                });

                // Add Identity services to the services container
                pipeline.UseErrorPage(ErrorPageOptions.ShowAll)
                        .UseStaticFile()
                        .UseDefaultIdentity<ApplicationDbContext, ApplicationUser, IdentityRole>(options =>
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
            });

            // Can this be built automagically?
            app.BuildPipeline();

            // TODO: not working above
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

        //services.SetupOptions<IdentityOptions>(options =>
        //        {
        //    options.Password.RequireDigit = false;
        //    options.Password.RequireLowercase = false;
        //    options.Password.RequireUppercase = false;
        //    options.Password.RequireNonLetterOrDigit = false;
        //    options.SecurityStampValidationInterval = TimeSpan.FromMinutes(20);
        //});
        //        services.SetupOptions<GoogleAuthenticationOptions>(options =>
        //        {
        //    options.ClientId = "514485782433-fr3ml6sq0imvhi8a7qir0nb46oumtgn9.apps.googleusercontent.com";
        //    options.ClientSecret = "V2nDD9SkFbvLTqAUBWBBxYAL";
        //});
        //        services.SetupOptions<FacebookAuthenticationOptions>(options =>
        //        {
        //    options.AppId = "901611409868059";
        //    options.AppSecret = "4aa3c530297b1dcebc8860334b39668b";
        //});
        //        services.SetupOptions<TwitterAuthenticationOptions>(options =>
        //        {
        //    options.ConsumerKey = "BSdJJ0CrDuvEhpkchnukXZBUv";
        //    options.ConsumerSecret = "xKUNuKhsRdHD03eLn67xhPAyE1wFFEndFo1X2UJaK2m1jdAxf4";
        //});

    }
}
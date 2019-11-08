using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BF.Server.Data;
using BF.Server.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using BF.Service.Prism.Events;
using BF.Service.Events;
using BF.Services.Prism.Events;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.AzureWebApps;
using Serilog.Exceptions;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;
using System.Security.Claims;
using BF.Services.Configuration;

namespace BF.Server {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {

            var loggerConfiguration = new LoggerConfiguration()
                                                .MinimumLevel.Verbose()
                                                .MinimumLevel.Override("Microsoft.ApplicationInsights", Serilog.Events.LogEventLevel.Warning)
                                                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                                                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                                                .Enrich.FromLogContext()
                                                .Enrich.WithExceptionDetails()
                                                //.WriteTo.Trace()
                                                //.WriteTo.Debug()
                                                .WriteTo.LiterateConsole();
            services.AddSingleton<IApplicationConfig>(Configuration.FromIConfiguration());
            services.AddRazorPages();
            services.AddSignalR();
            services.AddServerSideBlazor();
            services.AddTransient<HubConnectionBuilder>();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton<IBeerFactoryEventHandler, SignalRPrismBeerFactoryEventHandler>();



            services
                .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(
                    options => {
                        options.Realm = "BeerFactory";
                        options.Events = new BasicAuthenticationEvents {
                            OnValidatePrincipal = context => {
                                // TODO: Read this from configuration
                                if ((context.UserName == "raspberrypi") && (context.Password == "AoZ6WC7IMSBk!wq!e18NOCDI1!p6")) {
                                    var claims = new List<Claim> {
                                        new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                                    };

                                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                    context.Principal = principal;
                                } else if ((context.UserName == "server") && (context.Password == "AoZ6WC7IMSBk!wq!e18NOCDI1!p6")) {
                                    var claims = new List<Claim> {
                                        new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                                    };

                                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                    context.Principal = principal;
                                } else {
                                    // optional with following default.
                                    // context.AuthenticationFailMessage = "Authentication failed."; 
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

            Log.Logger = loggerConfiguration.CreateLogger();

            ConfigureBeerFactory(services);
        }

        private void ConfigureBeerFactory(IServiceCollection services) {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {

            loggerFactory.AddSerilog();
            var logger = loggerFactory.CreateLogger<Startup>();


            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHub<BFHub>("/bfHub");
            });

            HubConnectionHelper.Environment = "Server";
            HubConnectionHelper.Logger = loggerFactory.CreateLogger("HubConnectionHelper");
        }
    }
}

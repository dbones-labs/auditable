namespace Auditable.AspNetCore.Tests.Infrastructure
{
    using System;
    using Configuration;
    using global::Auditable.Tests;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _setupOverride;

        public CustomWebApplicationFactory(Action<IServiceCollection> setupOverride)
        {
            _setupOverride = setupOverride;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureAuditable(conf =>
                {
                    //this is registering the ASPNET dependencies
                    conf.Use<AspNet>();
                })
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseStartup<TStartup>().UseTestServer();
                });
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = DisabledAuthValues.Scheme;
                    options.DefaultChallengeScheme = DisabledAuthValues.Scheme;
                }).AddTestAuth(o => { });


                //You can do it this way!!!
                //this is setting up Auditable with ASPNET components
                //services.AddAuditableForAspNet();


                //Testing setupOverride
                services.Setup(_setupOverride);
 
            });
        }

        

    }
}
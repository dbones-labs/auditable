namespace Auditable.AspNetCore.Tests
{
    using System;
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
            var builder = Host.CreateDefaultBuilder()
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


                //this is setting up Auditable with ASPNET components
                services.AddAuditableForAspNet();


                //Testing setupOverride
                services.Setup(_setupOverride);
 
            });
        }

        

    }
}
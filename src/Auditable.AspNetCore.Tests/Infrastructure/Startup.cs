namespace Auditable.AspNetCore.Tests.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTelemetry.Trace;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOpenTelemetryTracerProvider(
                (builder) => builder
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
            );

            services
                .AddControllers(options =>
                {
                    //if(!securityConfiguration.Enabled) options.Filters.Add(new AllowAnonymousFilter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            //ensure its after auth*
            app.UseAuditable();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
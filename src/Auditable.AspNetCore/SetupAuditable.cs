namespace Auditable.AspNetCore
{
    using Collectors;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class SetupAuditable
    {
        public static IServiceCollection AddAuditableForAspNet(this IServiceCollection services)
        {
            //setup the base auditbale
            global::Auditable.SetupAuditable.AddAuditable(services);

            //override or add ASP.NET components
            //collectors
            services.AddScoped<IInitiatorCollector, ClaimsInitiatorCollector>();
            services.AddScoped<IRequestContextCollector, RequestContextCollector>();

            return services;
        }

        public static IApplicationBuilder UseAuditable(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestMiddleware>();
            app.UseMiddleware<InitiatorMiddleware>();
            return app;
        }
    }
}
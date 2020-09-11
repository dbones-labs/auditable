namespace Auditable
{
    using System;
    using Collectors;
    using Microsoft.Extensions.DependencyInjection;
    using Parsing;
    using Writers;

    public static class SetupAuditable
    {
        public static IServiceCollection AddAuditable(this IServiceCollection services)
        {
            //core
            services.AddTransient<IAuditable, Auditable>();
            services.AddScoped<IInternalAuditableContext, AuditableContext>();
            services.AddScoped<Func<IInternalAuditableContext>>(svc => svc.GetService<IInternalAuditableContext>);
            services.AddSingleton<IDifferenceEngine, DifferenceEngine>();
            services.AddSingleton<JsonSerializer>();
            services.AddSingleton<IAuditIdGenerator, AuditIdGenerator>();

            //collectors
            services.AddSingleton<IEnvironmentCollector, DefaultEnvironmentCollector>();
            services.AddScoped<IInitiatorCollector, PrincipalInitiatorCollector>();
            services.AddScoped<IRequestContextCollector, NullRequestContextCollector>();
            services.AddSingleton<IEntityIdCollector, EntityIdCollector>();

            //parse and write
            services.AddScoped<IParser, DefaultParser>();
            services.AddSingleton<IWriter, ConsoleWriter>();


            return services;
        }
    }
}
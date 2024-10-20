namespace Auditable.AspNetCore
{
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// this is recommended way to register the ASPNET dependencies.
    /// </summary>
    public class AspNet : IExtension
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAuditableForAspNetOnly();
        }
    }
}
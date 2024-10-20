namespace Auditable.Configuration
{
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// allows the extension to apply IoC setup/overrides
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// internal to setup the dependencies and options
        /// </summary>
        /// <param name="services">the IoC setup</param>
        void RegisterServices(IServiceCollection services);
    }
}
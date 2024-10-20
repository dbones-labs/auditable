namespace Auditable.Writers.File
{
    using System;
    using Configuration;
    using Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    public class File : IWriterProvider, ISetupOptions<FileWriterOptions>
    {
        Action<FileWriterOptions> _options = options => { };

        public void RegisterServices(IServiceCollection services)
        {
            Code.Require(()=> services != null, nameof(services));
            services.AddSingleton<IWriter, FileWriter>();
            services.Configure(_options);
        }

        public void Setup(Action<FileWriterOptions> options)
        {
            Code.Require(()=> options != null, nameof(options));
            _options = options;
        }
    }
}
namespace Auditable.Configuration
{
    using System.Collections.Generic;
    using Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    public class AuditableExtension
    {
        private IWriterProvider _writerProvider;
        private readonly List<IExtension> _extensions = new List<IExtension>();


        /// <summary>
        /// this is internal, to setup the IoC container
        /// </summary>
        /// <param name="services">IoC setup</param>
        internal void RegisterServices(IServiceCollection services)
        {
            Code.Require(()=> services!= null, nameof(services));
            _writerProvider?.RegisterServices(services);

            foreach (var extension in _extensions)
            {
                extension.RegisterServices(services);
            }
        }

        /// <summary>
        /// provide the writer you need to use, rember to see if it has
        /// a <see cref="ISetupOptions{T}"/>.Setup() to apply config options
        /// </summary>
        /// <typeparam name="T">The writers provider, which will be used to setup the writer</typeparam>
        /// <returns>the provider, note use this to access a Setup Function, some allow customizing</returns>
        public T UseWriter<T>() where T: IWriterProvider, new()
        {
            _writerProvider = new T();
            return (T)_writerProvider;
        }

        public T Use<T>() where T : IExtension, new()
        {
            var extension = new T();
            _extensions.Add(extension);
            return extension;
        }
    }
}
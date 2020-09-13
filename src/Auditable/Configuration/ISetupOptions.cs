namespace Auditable.Configuration
{
    using System;

    public interface ISetupOptions<out T> where T : class, new()
    {
        void Setup(Action<T> options);
    }
}
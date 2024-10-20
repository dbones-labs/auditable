namespace Auditable
{
    using System;
    using System.Threading.Tasks;

    public interface IAuditableContext : IDisposable, IAsyncDisposable
    {
        void WatchTargets(params object[] targets);
        void Removed<T>(string id);
        void Read<T>(string id);
        Task WriteLog();
    }
}
namespace Auditable
{
    using System.Threading.Tasks;

    public interface IAuditableContext
    {
        void WatchTargets(params object[] targets);
        void Removed<T>(string id);
        void Read<T>(string id);
        Task WriteLog();
    }
}
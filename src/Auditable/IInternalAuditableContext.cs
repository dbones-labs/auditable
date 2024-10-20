namespace Auditable
{
    public interface IInternalAuditableContext : IAuditableContext
    {
        void SetName(string name);
    }
}
namespace Auditable
{
    public interface IAuditable
    {
        IAuditableContext CreateContext(string name, params object[] targets);
    }
}
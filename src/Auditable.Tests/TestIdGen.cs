namespace Auditable.Tests
{
    public class TestIdGen : IAuditIdGenerator
    {
        public string GenerateId()
        {
            return "audit-id";
        }
    }
}
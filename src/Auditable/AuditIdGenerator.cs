namespace Auditable
{
    using CSharpVitamins;

    class AuditIdGenerator : IAuditIdGenerator
    {
        public string GenerateId()
        {
            return ShortGuid.NewGuid();
        }
    }
}
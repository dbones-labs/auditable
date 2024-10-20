namespace Auditable.Tests.Models.Complex
{
    public abstract class EntityRoot
    {
        public virtual string Id { get; set; }
        public virtual string Rev { get; set; }
    }
}
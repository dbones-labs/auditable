namespace Auditable.Collectors
{
    using System;

    public class NoIdAttributeException : Exception
    {
        public Type EntityType { get; }

        public NoIdAttributeException(Type entityType): base($"{entityType.Name} does not have an ID attribute")
        {
            EntityType = entityType;
        }
    }
}
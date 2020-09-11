namespace Auditable.Collectors
{
    using System;

    public class NoIdValueException : Exception
    {
        public Type EntityType { get; }
        public string IdAttributeName { get; }

        public NoIdValueException(Type entityType, string idAttributeName) : base($"{entityType.Name}.{idAttributeName} the id property had no value")
        {
            EntityType = entityType;
            IdAttributeName = idAttributeName;
        }
    }
}
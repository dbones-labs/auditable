namespace Auditable.Tests.Models.Complex
{
    using System;

    public class Edition
    {
        protected int _hash;

        protected Edition() { }

        public Edition(string name, EditionType editionType)
        {
            Name = name;
            Type = editionType;
            _hash = Name.GetHashCode() + Type.GetHashCode();
        }

        public virtual DateTime ReleaseDate { get; set; }
        public virtual string Name { get; private set; }
        public virtual EditionType Type { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as Edition;
            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hash;
        }
    }
}
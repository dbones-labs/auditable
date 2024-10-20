namespace Auditable.Tests.Models.Complex
{
    using System;

    public class Person : EntityRoot
    {
        protected Person() { }

        public Person(string name)
        {
            Name = name;
        }

        public virtual string Name { get; set; }

        public virtual DateTime BirthDate { get; set; }
    }
}
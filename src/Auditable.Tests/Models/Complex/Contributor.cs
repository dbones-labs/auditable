namespace Auditable.Tests.Models.Complex
{
    public class Contributor
    {
        protected Contributor() { }

        public Contributor(string id, string name, ContributorType contribution)
        {
            ContributorId = id;
            Name = name;
            Contribution = contribution;
        }

        public Contributor(Person person, ContributorType contribution)
            : this(person.Id, person.Name, contribution)
        {

        }

        public virtual string Name { get; private set; }
        public virtual ContributorType Contribution { get; private set; }
        public virtual string ContributorId { get; private set; }
    }
}
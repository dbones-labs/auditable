namespace Auditable.Tests.Models.Complex
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum ContributorType
    {
        Author,
        CoAuthor,
        Editor
    }

    public abstract class EntityRoot
    {
        public virtual string Id { get; set; }
        public virtual string Rev { get; set; }
    }

    public enum EditionType
    {
        HardBack,
        Paper,
        Electronic
    }

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



    public class Book : EntityRoot
    {
        private readonly IList<Contributor> _contributors = new List<Contributor>();
        private readonly IList<Edition> _editions = new List<Edition>();

        public Book() { }

        public Book(string title, Person author, int pages)
        {
            Title = title;
            NumberOfPages = pages;
            _contributors.Add(new Contributor(author, ContributorType.Author));
        }

        public virtual void AddContributor(Person contributor, ContributorType contributorType)
        {
            if (_contributors.Any(x => x.ContributorId == contributor.Id))
                return;

            _contributors.Add(new Contributor(contributor, contributorType));
        }

        public virtual void AddEdition(Edition edition)
        {
            if (_editions.Contains(edition))
            {
                _editions.Remove(edition);
            }
            _editions.Add(edition);
        }

        public virtual int NumberOfPages { get; private set; }
        public virtual string Title { get; private set; }
        public virtual IEnumerable<Contributor> Contributors => _contributors;
        public virtual IEnumerable<Edition> Editions => _editions;
    }
}
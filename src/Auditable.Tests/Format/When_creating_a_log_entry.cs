namespace Auditable.Tests.Format
{
    using System;
    using System.Collections.Generic;
    using global::Auditable.Parsing;
    using global::Auditable.Tests.Models.Simple;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Environment = global::Auditable.Collectors.Environment;

    [Subject("auditable")]
    public class When_creating_a_log_entry
    {
        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 3, 15, DateTimeKind.Utc));
            var container = ApplicationContainer.Build(configureAuditable: services => services.AddAuditable());
            var scope = container.CreateScope();
            var auditable = scope.ServiceProvider.GetService<IAuditable>();
            _writer = scope.ServiceProvider.GetService<TestWriter>();

            var person = new Person();
            _subject = auditable.CreateContext("Person.Created", person);
            person.Id = "123";
            person.Age = 38;
            person.Name = "Dave";
        };

        Because of = () => _subject.WriteLog().Await();

        It should_add_the_expected_log_entry = () =>
            Helpers.Compare(_writer.First.Deserialize(), _expeted, comparer => comparer.IgnoreMember("Delta"));

        static AuditableEntry _expeted => new AuditableEntry
        {
            Id = Helpers.AuditId,
            Action = "Person.Created",
            DateTime = SystemDateTime.UtcNow,
            Environment = new Environment
            {
                Host = Helpers.Host,
                Application = Helpers.Application
            },
            Initiator = null,
            Request = null,
            Targets = new List<AuditableTarget>
            {
                new AuditableTarget
                {
                    Id = null,
                    Audit = AuditType.Modified,
                    Style = ActionStyle.Observed,
                    Type = typeof(Person).FullName
                }
            }
        };

        static IAuditableContext _subject;
        static TestWriter _writer;
    }
}
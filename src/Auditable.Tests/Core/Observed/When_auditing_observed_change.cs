using Env = Auditable.Collectors.Environment;

namespace Auditable.Tests.Core.Observed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using global::Auditable.Parsing;
    using global::Auditable.Tests.Models.Simple;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using PowerAssert;

    [Subject("auditable")]
    public class When_auditing_observed_change
    {
        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 3, 15, DateTimeKind.Utc));
            var container = ApplicationContainer.Build(configureAuditable: services => services.AddAuditable());
            var scope = container.CreateScope();
            var auditable = scope.ServiceProvider.GetService<IAuditable>();
            _writer = scope.ServiceProvider.GetService<TestWriter>();

            var person = new Person
            {
                Id = "abc",
                Name = "Dave",
                Age = 24
            };

            _subject = auditable.CreateContext("Person.Read");
            _subject.WatchTargets(person);

            person.Age = 123;

        };

        Because of = () => _subject.WriteLog().Await();

        It should_have_a_target_with_audit_modified = () =>
            Helpers.Compare(_writer.First.Deserialize(), _expeted, comparer => comparer.IgnoreMember("Delta"));

        It should_have_a_deltas_entry =
            () => PAssert.IsTrue(() => _writer.First.Deserialize().Targets.First().Delta != null);

        It should_have_recorded_a_delta =
            () => Helpers.AssertDelta<Person, int>(_writer.First.Deserialize().Targets.First().Delta, p => p.Age, 24, 123);

        static AuditableEntry _expeted => new AuditableEntry
        {
            Id = Helpers.AuditId,
            Action = "Person.Read",
            DateTime = SystemDateTime.UtcNow,
            Environment = new Env
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
                    Id = "abc",
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
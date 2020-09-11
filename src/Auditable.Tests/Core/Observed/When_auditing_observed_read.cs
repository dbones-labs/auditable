using Env = Auditable.Extractors.Environment;

namespace Auditable.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Models.Simple;
    using Parsing;

    [Subject("auditable")]
    public class When_auditing_observed_read
    {
        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 3, 15, DateTimeKind.Utc));
            var container = ApplicationContainer.Build(configureAuditable: services => services.AddAuditable());
            var scope = container.CreateScope();
            var auditable = scope.ServiceProvider.GetService<IAuditable>();
            _writer = scope.ServiceProvider.GetService<TestWriter>();

            var person = new Person {
                Id = "abc",
                Name = "Dave",
                Age = 24
            }; 

            _subject = auditable.CreateContext("Person.Read");
            _subject.WatchTargets(person);
         
        };

        Because of = () => _subject.WriteLog().Await();

        It should_have_a_target_with_audit_read_and_no_delta = () => 
            Helpers.Compare(_writer.First.Deserialize(), _expeted);

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
                    Audit = AuditType.Read,
                    Style = ActionStyle.Observed,
                    Type = typeof(Person).FullName
                }
            }
        };

        static IAuditableContext _subject;
        static TestWriter _writer;
    }
}
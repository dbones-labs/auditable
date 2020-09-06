﻿using Env = Auditable.Extractors.Environment;

namespace Auditable.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Models.Simple;
    using Parsing;

    [Subject("auditable")]
    public class When_auditing_explicit_delete
    {
        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 3, 15, DateTimeKind.Utc));
            var container = ApplicationContainer.Build();
            var scope = container.CreateScope();
            var auditable = scope.ServiceProvider.GetService<IAuditable>();
            _writer = scope.ServiceProvider.GetService<TestWriter>();

            _subject = auditable.CreateContext("Person.Removed");
            _subject.Removed<Person>("123");
         
        };

        Because of = () => _subject.WriteLog().Await();

        It should_have_a_target_with_audit_removed_and_no_delta = () => 
            Helpers.Compare(_writer.First.Deserialize(), _expeted);

        static AuditableEntry _expeted => new AuditableEntry
        {
            Id = Helpers.AuditId,
            Action = "Person.Removed",
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
                    Id = "123",
                    Audit = AuditType.Removed,
                    Style = ActionStyle.Explicit,
                    Type = typeof(Person).FullName
                }
            }
        };

        static IAuditableContext _subject;
        static TestWriter _writer;
    }
}
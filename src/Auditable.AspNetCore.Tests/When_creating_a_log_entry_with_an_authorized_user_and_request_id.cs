namespace Auditable.AspNetCore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Collectors;
    using global::Auditable.Collectors;
    using global::Auditable.Collectors.Initiator;
    using global::Auditable.Collectors.Request;
    using global::Auditable.Infrastructure;
    using global::Auditable.Tests;
    using global::Auditable.Tests.Models.Simple;
    using Infrastructure;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Parsing;
    using Writers;
    using Environment = global::Auditable.Collectors.Environment.Environment;

    [Subject("auditable")]
    public class When_creating_a_log_entry_with_an_authorized_user_and_request_id
    {
        static CustomWebApplicationFactory<Startup> _factory;
        static TestWriter _writer;
        static HttpClient _client;


        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 3, 15, DateTimeKind.Utc));
            _writer = new TestWriter();
            _factory = new CustomWebApplicationFactory<Startup>(services =>
            {
                services.AddSingleton<IWriter>(_writer);
            });


            
            _client = _factory.CreateClient();
            //"traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01"
            _client.DefaultRequestHeaders.Add("traceparent", new []{ "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01" });
        };

        Because of = () => _client.GetAsync("/test").Await();

        It should_add_the_expected_log_entry = () =>
            Helpers.Compare(_writer.First.Deserialize(), _expeted, comparer =>
            {
                comparer.IgnoreMember("Delta");
                comparer.IgnoreMember("SpanId");
            });


        Cleanup after = () => _factory.Dispose(); 

        static AuditableEntry _expeted => new AuditableEntry
        {
            Id = Helpers.AuditId,
            Action = "test.get",
            DateTime = SystemDateTime.UtcNow,
            Environment = new Environment
            {
                Host = Helpers.Host,
                Application = Helpers.Application
            },
            Initiator = new Initiator
            {
                Id = "abc-123",
                Name = "dave"
            },
            Request = new RequestContext
            {
                ParentId = "00f067aa0ba902b7",
                TraceId = "4bf92f3577b34da6a3ce929d0e0e4736"
            },
            Targets = new List<AuditableTarget>
            {
                new AuditableTarget
                {
                    Id = "123",
                    Audit = AuditType.Read,
                    Style = ActionStyle.Explicit,
                    Type = typeof(Person).FullName
                }
            }
        };

    }
}

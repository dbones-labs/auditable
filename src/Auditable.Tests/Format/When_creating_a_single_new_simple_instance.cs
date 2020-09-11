namespace Auditable.Tests.Payload
{
    using System;
    using System.Linq;
    using Models.Simple;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using PowerAssert;

    [Subject("auditable")]
    public class When_writing_an_audit_entry
    {
        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 30, 15, DateTimeKind.Utc));
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

        It should_log_using_json_format = () => PAssert.IsTrue(() => _writer.Entries.Any(x=> x.Raw.Equals(entry)));

        static string entry =
            $"{{\"Action\":\"Person.Created\",\"DateTime\":\"1980-01-02T10:30:15Z\",\"Initiator\":null,\"Environment\":{{\"Host\":\"{Helpers.Host}\",\"Application\":\"{Helpers.Application}\"}},\"Request\":null,\"Targets\":[{{\"Type\":\"Auditable.Tests.Models.Simple.Person\",\"Id\":null,\"Delta\":{{\"Id\":[null,\"123\"],\"Name\":[null,\"Dave\"],\"Age\":[0,38]}},\"Style\":\"Observed\",\"Audit\":\"Modified\"}}],\"Id\":\"audit-id\"}}";
        static IAuditableContext _subject;
        static TestWriter _writer;
    }
}
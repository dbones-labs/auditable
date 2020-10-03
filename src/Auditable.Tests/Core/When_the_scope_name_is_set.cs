namespace Auditable.Tests.Core
{
    using System;
    using Configuration;
    using Infrastructure;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Models.Simple;
    using PowerAssert;

    [Subject("auditable")]
    public class When_the_scope_name_is_set
    {
        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 30, 15, DateTimeKind.Utc));
            var container = ApplicationContainer.Build(configureAuditable: services => services.AddAuditable());
            var scope = container.CreateScope();

            var auditable = scope.ServiceProvider.GetService<IAuditable>();
            _writer = scope.ServiceProvider.GetService<TestWriter>();

            //could have loaded it from the DB
            var person = new Person();
            person.Id = "123";
            person.Age = 38;
            person.Name = "Dave";

            //register object
            _subject = auditable.CreateContext("Person.Modified", person);
            person.Age = 21;
        };

        Because of = () => _subject.WriteLog().Await();

        It should_log_an_entry_with_the_action_name_set = () =>
            PAssert.IsTrue(() => _writer.First.Deserialize().Action == "Person.Modified");

        static IAuditableContext _subject;
        static TestWriter _writer;


    }
}
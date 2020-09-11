namespace Auditable.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Parsing;
    using Writers;
    using Machine.Specifications;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json.Linq;
    using ObjectsComparer;
    using PowerAssert;

    public static class ApplicationContainer
    {
        public static IServiceCollection Setup(this IServiceCollection serviceCollection, 
            Action<IServiceCollection> setup = null,
            Action<IServiceCollection> configureAuditable = null)
        {

            configureAuditable?.Invoke(serviceCollection);
            serviceCollection.AddSingleton<IWriter, TestWriter>();
            serviceCollection.AddSingleton(ctx => (TestWriter)ctx.GetService<IWriter>());
            serviceCollection.AddSingleton<IAuditIdGenerator, TestIdGen>();

            setup?.Invoke(serviceCollection);
            return serviceCollection;
        }

        public static IServiceProvider Build(
            Action<IServiceCollection> setup = null,
            Action<IServiceCollection> configureAuditable = null)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.Setup(setup, configureAuditable);
            return serviceCollection.BuildServiceProvider();
        }
    }


    //public class TestIn : IInitiatorCollector

    public class TestIdGen : IAuditIdGenerator
    {
        public string GenerateId()
        {
            return "audit-id";
        }
    }


    public class TestWriter : IWriter
    {
        public TestWriter()
        {
            Entries = new List<LogEntry>();
        }

        public List<LogEntry> Entries { get; protected set; }

        public LogEntry First => Entries.FirstOrDefault();

        public Task Write(string entry)
        {
            Entries.Add(new LogEntry(entry));
            return Task.CompletedTask;
        }
    }

    public static class Helpers
    {
        public static string AuditId => "audit-id";
        public static string Host => System.Environment.MachineName; 
        public static string Application => System.Reflection.Assembly.GetEntryAssembly().FullName;

        public static void Compare<T>(T left, T right, Action<ObjectsComparer.Comparer<T>> setup = null)
        {
            var comparer = new ObjectsComparer.Comparer<T>();
            setup?.Invoke(comparer);

            var different = comparer.Compare(left, right, out var differences);
            if (differences.Any())
            {
                throw new DifferentException(differences);
            }
        }

        public static bool AssertDelta<T, TValue>(JToken delta, Expression<Func<T, TValue>> property, object before, object after)
        {
            var path = FindMemberVisitor.GetMember(property);
            
            var entry = delta.SelectToken($"$.{path}");

            PAssert.IsTrue(()=> Equals(entry[0].Value<TValue>(), before));
            PAssert.IsTrue(() => Equals(entry[1].Value<TValue>(), after));

            return true;
        } 
    }

    public class DifferentException : Exception
    {
        public IEnumerable<Difference> Differences { get; }

        public DifferentException(IEnumerable<Difference> differences) 
            : base(string.Join(Environment.NewLine, differences.Select(x=>x.ToString())))
        {
            Differences = differences;
        }
    }

    public class LogEntry
    {
        public string Raw { get; }
        public JToken Json { get; }

        public T Deserialize<T>()
        {
            return new JsonSerializer().Deserialize<T>(Raw);
        }

        public AuditableEntry Deserialize()
        {
            var entry = Deserialize<AuditableEntry>();
            foreach (var target in entry.Targets)
            {
                if (target.Delta != null && target.Delta.HasValues) continue;
                target.Delta = null;
            }
            return entry;
        }

        public T GetValue<T>(string xpath)
        {
            return Json.SelectToken(xpath).Value<T>();
        }

        public LogEntry(string raw)
        {
            Raw = raw;
            Json = JToken.Parse(raw);
        }
    }

    public class ResetTheClock : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup()
        {
            SystemDateTime.Reset();
        }
    }


    internal class FindMemberVisitor : ExpressionVisitor
    {
        public static string GetMember(Expression linqExpression)
        {
            var visitor = new FindMemberVisitor();
            visitor.Visit(linqExpression);
            return visitor.ToString();
        }

        private readonly StringBuilder _memberName = new StringBuilder();
        private bool _isBody = false;
        private readonly string _alias;

        private FindMemberVisitor()
        {
        }

        public override string ToString()
        {
            return _memberName.ToString();
        }


        protected override Expression VisitMember(MemberExpression expression)
        {
            Visit(expression.Expression);
            if (_memberName.Length > 0)
            {
                _memberName.Append(".");
            }
            _memberName.AppendFormat(expression.Member.Name);

            return expression;
        }
    }


}

namespace Auditable.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Writers;

    public class TestWriter : IWriter
    {
        public TestWriter()
        {
            Entries = new List<LogEntry>();
        }

        public List<LogEntry> Entries { get; protected set; }

        public LogEntry First => Entries.FirstOrDefault();

        public Task Write(string id, string action, string entry)
        {
            Entries.Add(new LogEntry(entry));
            return Task.CompletedTask;
        }
    }
}
namespace Auditable.Tests
{
    using Newtonsoft.Json.Linq;
    using Parsing;

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
}
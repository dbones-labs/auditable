namespace Auditable
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Target
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public object Instance { get; set; }
        public string Before { get; set; }
        public JToken Delta { get; set; }
        public AuditType AuditType { get; set; }
        public ActionStyle ActionStyle { get; set; }
    }

    public class JsonSerializer
    {
        public string Serialize(object instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        public T Deserialize<T>(string payload)
        {
            return JsonConvert.DeserializeObject<T>(payload);
        }
    }

    public static class SystemDateTime
    {
        private static Func<DateTime> _getDateTime = () => DateTime.UtcNow;

        public static DateTime UtcNow => _getDateTime();

        public static void SetDateTime(Func<DateTime> getDateTime)
        {
            _getDateTime = getDateTime;
        }

        public static void Reset()
        {
            _getDateTime = () => DateTime.UtcNow;
        }
    }
}
namespace Auditable.Parsing
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    public class AuditableTarget
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public JToken Delta { get; set; }

        [JsonConverter(typeof(StringEnumConverter))] 
        public ActionStyle Style { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public AuditType Audit { get; set; }
    }
}
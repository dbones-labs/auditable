namespace Auditable
{
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
}
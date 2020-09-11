namespace Auditable.Extractors
{
    public class RequestContext
    {
        public string SpanId { get; set; }
        public string TraceId { get; set; }
        public string ParentId { get; set; }
    }
}
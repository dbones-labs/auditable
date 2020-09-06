namespace Auditable.Extractors
{
    public class RequestContext
    {
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
    }
}
namespace Auditable.AspNetCore.Collectors
{
    using System.Threading.Tasks;
    using global::Auditable.Collectors;
    using global::Auditable.Collectors.Request;

    class RequestContextCollector : IRequestContextCollector
    {
        public RequestContext RequestContext { get; set; }

        public Task<RequestContext> Extract()
        {
            return Task.FromResult(RequestContext);
        }
    }
}
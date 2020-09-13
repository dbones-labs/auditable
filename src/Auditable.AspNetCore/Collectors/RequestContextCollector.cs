namespace Auditable.AspNetCore.Collectors
{
    using System.Threading.Tasks;
    using global::Auditable.Collectors;

    class RequestContextCollector : IRequestContextCollector
    {
        public RequestContext RequestContext { get; set; }

        public Task<RequestContext> Extract()
        {
            return Task.FromResult(RequestContext);
        }
    }
}
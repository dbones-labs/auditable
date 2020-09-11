namespace Auditable.AspNetCore
{
    using System.Threading.Tasks;
    using Collectors;

    class RequestContextCollector : IRequestContextCollector
    {
        public RequestContext RequestContext { get; set; }

        public Task<RequestContext> Extract()
        {
            return Task.FromResult(RequestContext);
        }
    }
}
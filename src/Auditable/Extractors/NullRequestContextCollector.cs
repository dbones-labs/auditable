namespace Auditable.Extractors
{
    using System.Threading.Tasks;

    public class NullRequestContextCollector : IRequestContextCollector
    {
        public Task<RequestContext> Extract()
        {
            return Task.FromResult<RequestContext>(null);
        }
    }
}
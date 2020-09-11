namespace Auditable.Collectors
{
    using System.Threading;
    using System.Threading.Tasks;

    public class PrincipalInitiatorCollector : IInitiatorCollector
    {
        public Task<Initiator> Extract()
        {
            var principal = Thread.CurrentPrincipal;
            if (principal == null) return Task.FromResult<Initiator>(null);
            var initiator = new Initiator()
            {
                Id = principal.Identity?.Name
            };
            return Task.FromResult(initiator);
        }
    }
}
namespace Auditable.Collectors.Initiator
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Gets the current Principal, note this will need to be set, otherwise it will be null
    /// </summary>
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
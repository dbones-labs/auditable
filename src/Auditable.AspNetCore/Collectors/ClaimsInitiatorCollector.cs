namespace Auditable.AspNetCore
{
    using System.Threading.Tasks;
    using Collectors;

    class ClaimsInitiatorCollector : IInitiatorCollector
    {
        public Initiator Initiator { get; set; }

        public Task<Initiator> Extract()
        {
            return Task.FromResult(Initiator);
        }
    }
}
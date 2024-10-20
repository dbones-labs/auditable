﻿namespace Auditable.AspNetCore.Collectors
{
    using System.Threading.Tasks;
    using global::Auditable.Collectors;
    using global::Auditable.Collectors.Initiator;

    class ClaimsInitiatorCollector : IInitiatorCollector
    {
        public Initiator Initiator { get; set; }

        public Task<Initiator> Extract()
        {
            return Task.FromResult(Initiator);
        }
    }
}
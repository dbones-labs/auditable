namespace Auditable.Parsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Collectors;

    public class DefaultParser : IParser
    {
        private readonly IInitiatorCollector _initiatorCollector;
        private readonly IEnvironmentCollector _environmentCollector;
        private readonly IRequestContextCollector _requestContextCollector;
        private readonly IEntityIdCollector _entityIdCollector;
        private readonly JsonSerializer _serializer;

        public DefaultParser(
            IInitiatorCollector initiatorCollector,
            IEnvironmentCollector environmentCollector,
            IRequestContextCollector requestContextCollector,
            IEntityIdCollector entityIdCollector,
            JsonSerializer serializer)
        {
            _initiatorCollector = initiatorCollector;
            _environmentCollector = environmentCollector;
            _requestContextCollector = requestContextCollector;
            _entityIdCollector = entityIdCollector;
            _serializer = serializer;
        }

        public async Task<string> Parse(string id, string actionName, IEnumerable<Target> targets)
        {
            var payload = new AuditableEntry()
            {
                Id = id,
                Action = actionName,
                DateTime = SystemDateTime.UtcNow,
                
                Environment = await _environmentCollector.Extract(),
                Initiator = await _initiatorCollector.Extract(),
                Request = await _requestContextCollector.Extract(),
                Targets = targets.Select(x=> new AuditableTarget()
                {
                    Delta = x.Delta,
                    Id = x.Id ?? _entityIdCollector.Extract(x),
                    Type = x.Type,
                    Style = x.ActionStyle,
                    Audit = x.ActionStyle == ActionStyle.Explicit 
                        ? x.AuditType
                        : x.Delta == null ? AuditType.Read : AuditType.Modified
                })
            };

            return _serializer.Serialize(payload);
        }
    }

    
}
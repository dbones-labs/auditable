namespace Auditable
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CSharpVitamins;
    using Extractors;
    using Parsing;
    using Writers;

    public class AuditableContext : IInternalAuditableContext
    {
        private readonly IParser _parser;
        private readonly IEntityIdCollector _entityIdCollector;
        private readonly JsonSerializer _serializer;
        private readonly IDifferenceEngine _engine;
        private readonly IWriter _writer;
        private readonly IAuditIdGenerator _auditIdGenerator;
        private string _name;
        private readonly Dictionary<string, Target> _targets = new Dictionary<string, Target>();

        public AuditableContext(
            IParser parser,
            IEntityIdCollector entityIdCollector,
            JsonSerializer serializer,
            IDifferenceEngine engine,
            IWriter writer,
            IAuditIdGenerator auditIdGenerator
        )
        {
            _parser = parser;
            _entityIdCollector = entityIdCollector;
            _serializer = serializer;
            _engine = engine;
            _writer = writer;
            _auditIdGenerator = auditIdGenerator;
        }

        public void WatchTargets(params object[] targets)
        {

            foreach (var instance in targets)
            {
                var copy = _serializer.Serialize(instance);
                var id = _entityIdCollector.Extract(instance);
                var type = instance.GetType();
                var key = GetKey(type, id);

                var target = new Target()
                {
                    Type = type.FullName,
                    Id = id,
                    Before = copy,
                    Instance = instance,
                    ActionStyle = ActionStyle.Observed
                };

                _targets.Add(key , target);
            }
        }

        public void Removed<T>(string id)
        {
            SetExplicateAction<T>(id, AuditType.Removed);
        }

        public void Read<T>(string id)
        {
            SetExplicateAction<T>(id, AuditType.Read);
        }

        public void SetExplicateAction<T>(string id, AuditType action)
        {
            var type = typeof(T);
            var key = GetKey(type, id);

            if (!_targets.TryGetValue(key, out var target))
            {
                target = new Target()
                {
                    Type = type.FullName,
                    Id = id
                };
                _targets.Add(key, target);
            }

            target.ActionStyle = ActionStyle.Explicit;
            target.AuditType = action;
         }

        public async Task WriteLog()
        {
            foreach (var target in _targets.Values)
            {
                if(target.ActionStyle == ActionStyle.Explicit) continue;
                var possibleChange = _serializer.Serialize(target.Instance);
                var diff = _engine.Differences(target.Before, possibleChange);
                target.Delta = diff;
            }

            var parsedOutput = await _parser.Parse(_auditIdGenerator.GenerateId(), _name, _targets.Values);
            await _writer.Write(parsedOutput);
        }

        public void SetName(string name)
        {
            _name = name;
        }

        private string GetKey(Type type, string id)
        {
            return $"{type.FullName}-{id}";
        }
    }

    public interface IAuditIdGenerator
    { 
        string GenerateId();
    }

    class AuditIdGenerator : IAuditIdGenerator
    {
        public string GenerateId()
        {
            return ShortGuid.NewGuid();
        }
    }
}
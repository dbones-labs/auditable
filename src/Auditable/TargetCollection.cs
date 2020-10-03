namespace Auditable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Collectors.EntityId;
    using Infrastructure;

    internal class TargetCollection : IEnumerable<Target>
    {
        private readonly IEntityIdCollector _idCollector;
        private readonly Dictionary<object, Target> _instanceLookup = new Dictionary<object, Target>();
        private readonly Dictionary<string, Target> _idLookup = new Dictionary<string, Target>();
  
        public TargetCollection(IEntityIdCollector idCollector)
        {
            _idCollector = idCollector;
        }

        public bool TryGet(object instance, Type type, string id, out Target target)
        {
            var confirmedId = id ?? _idCollector.Extract(instance);
            return string.IsNullOrEmpty(confirmedId)
                ? TryGet(instance, out target)
                : TryGet(type, id, out target);
        }

        public bool TryGet(object instance, out Target target)
        {
            Code.Require(() => instance != null, nameof(target));
            return _instanceLookup.TryGetValue(instance, out target);
        }

        public bool TryGet(Type type, string id, out Target target)
        {
            Code.Require(() => type != null, nameof(type));
            Code.Require(()=> !string.IsNullOrEmpty(id), nameof(id));

            var key = GetKey(type, id);
            return _idLookup.TryGetValue(key, out target);
        }

        private string GetKey(Type type, string id)
        {
            return $"{type.FullName}-{id}";
        }

        public void Add(Type type, string id, Target target)
        {
            Code.Require(() => type != null, nameof(type));
            Code.Require(() => !string.IsNullOrEmpty(id), nameof(id));
            Code.Require(() => target != null, nameof(target));

            var key = GetKey(type, id);

            if (_idLookup.ContainsKey(key))
            {
                return;
            }

            _idLookup.Add(key, target);
        }


        public void Add(object instance, Target target)
        {
            Code.Require(() => instance != null, nameof(instance));
            Code.Require(() => target != null, nameof(target));

            var id = _idCollector.Extract(instance);
            var hasId = id != null;
            if (hasId)
            {
                var type = instance.GetType();
                Add(type, id, target);
                return;
            }

            var processedInstance = _instanceLookup.ContainsKey(instance);
            if (processedInstance)
            {
                return;
            }

            _instanceLookup.Add(instance, target);
        }

        public IEnumerator<Target> GetEnumerator()
        { 
            var items = _idLookup.Values.Union(_instanceLookup.Values);
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
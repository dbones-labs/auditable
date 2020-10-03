namespace Auditable
{
    using System;
    using Infrastructure;

    public class Auditable : IAuditable
    {
        private readonly Func<IInternalAuditableContext> _contextCtor;

        public Auditable(Func<IInternalAuditableContext> contextCtor)
        {
            _contextCtor = contextCtor;
        }

        public IAuditableContext CreateContext(string name, params object[] targets)
        {
            Code.Require(()=> name != null, nameof(name));

            var context = _contextCtor();
            context.SetName(name);
            context.WatchTargets(targets);

            return context;
        }
    }
}
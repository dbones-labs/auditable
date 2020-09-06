namespace Auditable
{
    using System;

    public class Auditable : IAuditable
    {
        private readonly Func<IInternalAuditableContext> _contextCtor;

        public Auditable(Func<IInternalAuditableContext> contextCtor)
        {
            _contextCtor = contextCtor;
        }

        public IAuditableContext CreateContext(string name, params object[] targets)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var context = _contextCtor();
            context.SetName(name);
            context.WatchTargets(targets);

            return context;
        }
    }
}
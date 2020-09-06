namespace Auditable.Parsing
{
    using System;
    using System.Collections.Generic;
    using Extractors;
    using Environment = Extractors.Environment;

    public class AuditableEntry
    {
        public string Action { get; set; }
        public DateTime DateTime { get; set; }
        public Initiator Initiator { get; set; }
        public Environment Environment { get; set; }
        public RequestContext Request { get; set; }
        public IEnumerable<AuditableTarget> Targets { get; set; }
        public string Id { get; set; }
    }
}
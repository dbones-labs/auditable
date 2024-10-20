namespace Auditable.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ObjectsComparer;

    public class DifferentException : Exception
    {
        public IEnumerable<Difference> Differences { get; }

        public DifferentException(IEnumerable<Difference> differences) 
            : base(string.Join(Environment.NewLine, differences.Select(x=>x.ToString())))
        {
            Differences = differences;
        }
    }
}
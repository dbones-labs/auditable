namespace Auditable.Writers.Console
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;

    public class ConsoleWriter : IWriter
    {
        public Task Write(string id, string action, string entry)
        {
            Code.Require(()=> !string.IsNullOrEmpty(entry), nameof(entry));
            Console.WriteLine(entry);
            return Task.CompletedTask;
        }
    }
}
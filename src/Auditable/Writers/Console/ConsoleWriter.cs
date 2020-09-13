namespace Auditable.Writers.Console
{
    using System;
    using System.Threading.Tasks;

    public class ConsoleWriter : IWriter
    {
        public Task Write(string id, string action, string entry)
        {
            Console.WriteLine(entry);
            return Task.CompletedTask;
        }
    }
}
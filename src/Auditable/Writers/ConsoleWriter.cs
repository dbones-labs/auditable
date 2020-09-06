namespace Auditable.Writers
{
    using System;
    using System.Threading.Tasks;

    public class ConsoleWriter : IWriter
    {
        public Task Write(string entry)
        {
            Console.WriteLine(entry);
            return Task.CompletedTask;
        }
    }
}
namespace Auditable.Writers
{
    using System.IO;
    using System.Threading.Tasks;

    public class FileWriter : IWriter
    {
        public Task Write(string entry)
        {
            File.WriteAllText("", entry);
            return Task.CompletedTask;
        }
    }
}
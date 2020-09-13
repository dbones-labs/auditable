namespace Auditable.Writers.File
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public class FileWriter : IWriter
    {
        private readonly IOptions<FileWriterOptions> _options;

        public FileWriter(IOptions<FileWriterOptions> options)
        {
            _options = options;
        }

        public Task Write(string id, string action, string entry)
        {
            var file = _options.Value.GetFileName(id, action);
            var folder = _options.Value.Folder;
            System.IO.File.WriteAllText(Path.Combine(folder, file), entry);
            return Task.CompletedTask;
        }
    }
}
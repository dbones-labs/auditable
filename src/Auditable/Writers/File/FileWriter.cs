namespace Auditable.Writers.File
{
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.Extensions.Options;

    public class FileWriter : IWriter
    {
        private readonly IOptions<FileWriterOptions> _options;

        public FileWriter(IOptions<FileWriterOptions> options)
        {
            Code.Require(()=> options != null, nameof(options));
            _options = options;
        }

        public Task Write(string id, string action, string entry)
        {
            Code.Require(() => !string.IsNullOrEmpty(id), nameof(id));
            Code.Require(() => !string.IsNullOrEmpty(action), nameof(action));
            Code.Require(() => !string.IsNullOrEmpty(entry), nameof(entry));

            var file = _options.Value.GetFileName(id, action);
            var folder = _options.Value.Folder;
            var path = Path.Combine(folder, file);
            System.IO.File.WriteAllText(path, entry);
            return Task.CompletedTask;
        }
    }
}
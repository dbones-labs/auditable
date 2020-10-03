namespace Auditable.Writers.File
{
    using System.IO;
    using Infrastructure;

    public class FileWriterOptions
    {
        public GetFileName GetFileName { get; set; } = (id, action) =>
        {
            Code.Require(()=> !string.IsNullOrEmpty(id), nameof(id));
            var date = SystemDateTime.UtcNow.ToString("yyyy-MM-dd-H-mm-ss");
            return $"{date}_{id}.auditable";
        };
        public string Folder { get; set; } = Directory.GetCurrentDirectory();
    }
}
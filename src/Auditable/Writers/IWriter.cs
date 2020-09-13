namespace Auditable.Writers
{
    using System.Threading.Tasks;

    public interface IWriter
    {
        Task Write(string id, string action, string entry);
    }
}
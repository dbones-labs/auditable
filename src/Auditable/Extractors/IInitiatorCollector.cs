namespace Auditable.Extractors
{
    using System.Threading.Tasks;

    public interface IInitiatorCollector
    {
        Task<Initiator> Extract();
    }
}
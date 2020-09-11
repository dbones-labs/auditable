namespace Auditable.Collectors
{
    using System.Threading.Tasks;

    public interface IInitiatorCollector
    {
        Task<Initiator> Extract();
    }
}
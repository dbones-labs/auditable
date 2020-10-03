namespace Auditable.Collectors
{
    using System.Threading.Tasks;

    /// <summary>
    /// Grab information about the user who is acting the action
    /// </summary>
    public interface IInitiatorCollector
    {
        Task<Initiator> Extract();
    }
}
namespace Auditable.Collectors
{
    using System.Threading.Tasks;

    /// <summary>
    /// Grab information about the running environment
    /// </summary>
    public interface IEnvironmentCollector
    {
        Task<Environment> Extract();
    }
}
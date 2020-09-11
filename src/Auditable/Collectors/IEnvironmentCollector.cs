namespace Auditable.Collectors
{
    using System.Threading.Tasks;

    public interface IEnvironmentCollector
    {
        Task<Environment> Extract();
    }
}
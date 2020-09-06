namespace Auditable.Extractors
{
    using System.Threading.Tasks;

    public interface IEnvironmentCollector
    {
        Task<Environment> Extract();
    }
}
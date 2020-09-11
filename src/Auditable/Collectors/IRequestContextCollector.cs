namespace Auditable.Collectors
{
    using System.Threading.Tasks;

    public interface IRequestContextCollector
    {
        Task<RequestContext> Extract();
    }
}
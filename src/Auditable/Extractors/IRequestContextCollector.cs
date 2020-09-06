namespace Auditable.Extractors
{
    using System.Threading.Tasks;

    public interface IRequestContextCollector
    {
        Task<RequestContext> Extract();
    }
}
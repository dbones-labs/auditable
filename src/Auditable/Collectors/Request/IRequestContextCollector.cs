namespace Auditable.Collectors.Request
{
    using System.Threading.Tasks;

    /// <summary>
    /// grab information about the request (tracing)
    /// </summary>
    public interface IRequestContextCollector
    {
        Task<RequestContext> Extract();
    }
}
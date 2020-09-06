namespace Auditable.Extractors
{
    public interface IEntityIdCollector
    {
        string Extract(object instance);
    }
}
namespace Auditable.Collectors
{
    public interface IEntityIdCollector
    {
        string Extract(object instance);
    }
}
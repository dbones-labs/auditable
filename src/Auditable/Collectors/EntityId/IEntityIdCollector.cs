namespace Auditable.Collectors
{
    /// <summary>
    /// get the id from the root entity that we are watching
    /// </summary>
    public interface IEntityIdCollector
    {
        string Extract(object instance);
    }
}
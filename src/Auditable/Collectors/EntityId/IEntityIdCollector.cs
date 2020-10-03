namespace Auditable.Collectors.EntityId
{
    /// <summary>
    /// get the id from the root entity that we are watching
    /// </summary>
    public interface IEntityIdCollector
    {
        /// <summary>
        /// get the id for the root identity
        /// </summary>
        /// <param name="instance">root object, which will have the ID</param>
        /// <returns>the id, or null, when its a new object that will have no Id set</returns>
        string Extract(object instance);
    }
}
namespace Auditable.Delta
{
    using Newtonsoft.Json.Linq;

    public interface IDifferenceEngine
    {
        JToken Differences(string left, string right);
    }
}
namespace Auditable.Infrastructure
{
    using Newtonsoft.Json;

    public class JsonSerializer
    {
        public string Serialize(object instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        public T Deserialize<T>(string payload)
        {
            return JsonConvert.DeserializeObject<T>(payload);
        }
    }
}
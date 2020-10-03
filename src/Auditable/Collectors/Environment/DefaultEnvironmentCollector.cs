namespace Auditable.Collectors.Environment
{
    using System.Reflection;
    using System.Threading.Tasks;

    public class DefaultEnvironmentCollector : IEnvironmentCollector
    {
        public Task<Environment> Extract()
        {
            var env = new Environment()
            {
                Application = Assembly.GetEntryAssembly().FullName,
                Host = System.Environment.MachineName
            };
            return Task.FromResult(env);
        }
    }
}
namespace Auditable.AspNetCore.Tests
{
    using System.Threading.Tasks;
    using global::Auditable.Tests.Models.Simple;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("/test")]
    [Authorize]
    public class TestController :  Controller
    {
        private readonly IAuditable _auditable;
        private readonly ILogger<TestController> _logger;

        public TestController(
            IAuditable auditable,
            ILogger<TestController> logger)
        {
            _auditable = auditable;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            await using var auditContext = _auditable.CreateContext("test.get");
            auditContext.Read<Person>("123");

            _logger.LogInformation("called the get method, and did some awesome things");

            return new OkResult();
        }
    }
}
namespace Auditable.AspNetCore
{
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Collectors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public class InitiatorMiddleware
    {
        private readonly RequestDelegate _next;

        public InitiatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var initiatorCollector = context.RequestServices.GetService<IInitiatorCollector>() as ClaimsInitiatorCollector;
            if (initiatorCollector == null)
            {
                throw new ClaimsInitiatorNotRegisterException();
            }
            var user = context.User;
            if (user != null)
            {
                //https://www.jerriepelser.com/blog/authenticate-oauth-aspnet-core-2/
                initiatorCollector.Initiator = new Initiator
                {
                    Id =  user.FindFirstValue(ClaimTypes.NameIdentifier), 
                    Name=  user.FindFirstValue(ClaimTypes.Name) 
                };
            }

            await _next(context);
        }
    }
}

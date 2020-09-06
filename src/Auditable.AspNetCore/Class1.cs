namespace Auditable.AspNetCore
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Extractors;
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
            var initiatorCollector = context.RequestServices.GetService<IInitiatorCollector>() as ClaimsInitiator;
            if (initiatorCollector == null)
            {
                throw new ClaimsInitiatorNotRegisterException();
            }
            var user = context.User;
            if (user != null)
            { 
                initiatorCollector.Initiator = new Initiator
                {
                    Id = user.FindFirstValue(ClaimTypes.NameIdentifier)
                };
            }

            await _next(context);
        }
    }

    public class ClaimsInitiatorNotRegisterException : Exception
    {
    }


    class ClaimsInitiator : IInitiatorCollector
    {
        public Initiator Initiator { get; set; }

        public Task<Initiator> Extract()
        {
            return Task.FromResult(Initiator);
        }
    }
}

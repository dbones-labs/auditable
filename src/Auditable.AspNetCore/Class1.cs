namespace Auditable.AspNetCore
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
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


    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestCollector = context.RequestServices.GetService<IRequestContextCollector>() as RequestContextCollector;
            if (requestCollector == null)
            {
                throw new RequestContextCollectorNotRegisterException();
            }

            //https://www.w3.org/TR/trace-context/#trace-context-http-headers-format
            //curl --header "traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01" localhost:8080
            var hasW3cTraceParent = context.Request.Headers.ContainsKey("traceparent");
            var hasActivity = Activity.Current != null;
            if (!hasW3cTraceParent || !hasActivity) await _next(context);

            //var raw = context.Request.Headers["traceparent"].FirstOrDefault()?.Split("-");
            //if (raw == null) await _next(context);

            var activity = Activity.Current;
            
            //var version = raw[0];
            //var trace = raw[1];
            //var span = raw[2];
            //var flags = raw[3];

            


            requestCollector.RequestContext = new RequestContext()
            {
                SpanId = activity.SpanId.ToString(),
                TraceId = activity.TraceId.ToString(),
                ParentId = activity.ParentSpanId.ToString()
            };


            
        

            await _next(context);
        }
    }



    public static class SetupAuditable
    {
        public static IServiceCollection AddAuditable(this IServiceCollection services)
        {
            //setup the base auditbale
            global::Auditable.SetupAuditable.AddAuditable(services);

            //override or add ASP.NET components
            //collectors
            services.AddScoped<IInitiatorCollector, ClaimsInitiatorCollector>();
            services.AddScoped<IRequestContextCollector, RequestContextCollector>();

            return services;
        }
    }

    public class ClaimsInitiatorNotRegisterException : Exception
    {
    }

    public class RequestContextCollectorNotRegisterException : Exception
    {
    }

    class RequestContextCollector : IRequestContextCollector
    {
        public RequestContext RequestContext { get; set; }

        public Task<RequestContext> Extract()
        {
            return Task.FromResult(RequestContext);
        }
    }


    class ClaimsInitiatorCollector : IInitiatorCollector
    {
        public Initiator Initiator { get; set; }

        public Task<Initiator> Extract()
        {
            return Task.FromResult(Initiator);
        }
    }
}

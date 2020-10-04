namespace Auditable.AspNetCore.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Collectors;
    using global::Auditable.Collectors.Request;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

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
            
            //simple check for the header.
            var hasW3cTraceParent = context.Request.Headers.ContainsKey("traceparent");
            var hasActivity = Activity.Current != null;
            
            if (!hasW3cTraceParent || !hasActivity)
            {
                await _next(context);
                return;
            }

            //use the Activity set by the OpenTelemetry Lib
            var activity = Activity.Current;
            
            requestCollector.RequestContext = new RequestContext
            {
                SpanId = activity.SpanId.ToString(),
                TraceId = activity.TraceId.ToString(),
                ParentId = activity.ParentSpanId.ToString()
            };

            await _next(context);
        }
    }
}
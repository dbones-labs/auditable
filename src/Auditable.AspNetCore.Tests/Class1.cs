namespace Auditable.AspNetCore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Extractors;
    using global::Auditable.Tests;
    using OpenTelemetry.Trace;
    using global::Auditable.Tests.Models.Simple;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Parsing;
    using Writers;
    using Environment = global::Auditable.Extractors.Environment;

    [Subject("auditable")]
    public class When_creating_a_log_entry_with_an_authorized_user_and_request_id
    {
        static CustomWebApplicationFactory<Startup> _factory;
        static TestWriter _writer;
        static HttpClient _client;


        private Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 3, 15, DateTimeKind.Utc));
            _writer = new TestWriter();
            _factory = new CustomWebApplicationFactory<Startup>(services =>
            {
                services.AddSingleton<IWriter>(_writer);
            });


            
            _client = _factory.CreateClient();
            //"traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01"
            _client.DefaultRequestHeaders.Add("traceparent", new []{ "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01" });
        };

        Because of = () => _client.GetAsync("/test").Await();

        It should_add_the_expected_log_entry = () =>
            Helpers.Compare(_writer.First.Deserialize(), _expeted, comparer =>
            {
                comparer.IgnoreMember("Delta");
                comparer.IgnoreMember("SpanId");
            });


        Cleanup after = () => _factory.Dispose(); 

        static AuditableEntry _expeted => new AuditableEntry
        {
            Id = Helpers.AuditId,
            Action = "test.get",
            DateTime = SystemDateTime.UtcNow,
            Environment = new Environment
            {
                Host = Helpers.Host,
                Application = Helpers.Application
            },
            Initiator = new Initiator
            {
                Id = "abc-123",
                Name = "dave"
            },
            Request = new RequestContext
            {
                ParentId = "00f067aa0ba902b7",
                TraceId = "4bf92f3577b34da6a3ce929d0e0e4736"
            },
            Targets = new List<AuditableTarget>
            {
                new AuditableTarget
                {
                    Id = "123",
                    Audit = AuditType.Read,
                    Style = ActionStyle.Explicit,
                    Type = typeof(Person).FullName
                }
            }
        };

    }



    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _setupOverride;

        public CustomWebApplicationFactory(Action<IServiceCollection> setupOverride)
        {
            _setupOverride = setupOverride;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseStartup<TStartup>().UseTestServer();
                });
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = DisabledAuthValues.Scheme;
                    options.DefaultChallengeScheme = DisabledAuthValues.Scheme;
                }).AddTestAuth(o => { });


                //this is setting up Auditable with ASPNET components
                services.AddAuditable();


                //Testing setupOverride
                services.Setup(_setupOverride);
 
            });
        }

        

    }

    public static class Setup
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(DisabledAuthValues.Scheme, DisabledAuthValues.Authority, configureOptions);

        }
    }


    public static class DisabledAuthValues
    {
        public const string Scheme = "disabled scheme";
        public const string Authority = "disabled auth";

    }

    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authenticationTicket = new AuthenticationTicket(
                new ClaimsPrincipal(Options.Identity),
                new AuthenticationProperties(),
                DisabledAuthValues.Scheme);

            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }

    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new[]
        {
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "abc-123"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "dave")
        }, "test");
    }

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

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOpenTelemetryTracerProvider(
                (builder) => builder
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
            );

            services
                .AddControllers(options =>
                {
                    //if(!securityConfiguration.Enabled) options.Filters.Add(new AllowAnonymousFilter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            //add middleware
            app.UseMiddleware<RequestMiddleware>();
            app.UseMiddleware<InitiatorMiddleware>();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

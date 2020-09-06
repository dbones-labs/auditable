using System;

namespace Auditable.AspNetCore.Tests
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading;
    using System.Threading.Tasks;
    using Extractors;
    using global::Auditable.Tests;
    using global::Auditable.Tests.Models.Simple;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Parsing;

    [Subject("auditable")]
    public class When_creating_a_log_entry
    {
        static CustomWebApplicationFactory<Startup> _factory;
        static IAuditableContext _subject;
        static TestWriter _writer;
        
        Establish context = () =>
        {
            SystemDateTime.SetDateTime(() => new DateTime(1980, 01, 02, 10, 3, 15, DateTimeKind.Utc));
            _factory = new CustomWebApplicationFactory<Startup>();



            
            
        };

        Because of = () => _subject.WriteLog().Await();

        It should_add_the_expected_log_entry = () =>
            Helpers.Compare(_writer.First.Deserialize(), _expeted, comparer => comparer.IgnoreMember("Delta"));


        Cleanup after = () => _factory.Dispose(); 

        static AuditableEntry _expeted => new AuditableEntry
        {
            Id = Helpers.AuditId,
            Action = "Person.Created",
            DateTime = SystemDateTime.UtcNow,
            Environment = new Environment
            {
                Host = Helpers.Host,
                Application = Helpers.Application
            },
            Initiator = null,
            Request = null,
            Targets = new List<AuditableTarget>
            {
                new AuditableTarget
                {
                    Id = null,
                    Audit = AuditType.Modified,
                    Style = ActionStyle.Observed,
                    Type = typeof(Person).FullName
                }
            }
        };

    }



    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = DisabledAuthValues.Scheme;
                    options.DefaultChallengeScheme = DisabledAuthValues.Scheme;
                }).AddTestAuth(o => { });



                var container = ApplicationContainer.Build();
                var scope = container.CreateScope();
                var auditable = scope.ServiceProvider.GetService<IAuditable>();
                _writer = scope.ServiceProvider.GetService<TestWriter>();

                var person = new Person();
                person.Id = "123";
                person.Age = 38;
                person.Name = "Dave";

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
        public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new Claim[]
        {
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Guid.NewGuid().ToString()),
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
            var auditContext = _auditable.CreateContext("test.get");
            auditContext.Read<Person>("123");

            await auditContext.WriteLog();
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
            services
                .AddControllers(options =>
                {
                    //if(!securityConfiguration.Enabled) options.Filters.Add(new AllowAnonymousFilter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //add middleware
            app.UseMiddleware<InitiatorMiddleware>();
     
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

     

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

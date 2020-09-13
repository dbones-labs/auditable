# auditable

This is a small auditing framework

The need for auditing already exists in projects, the requirement to log information to prove something happened, but also to capture who, what and when a business action happened.

## Features

- `Unit of work style` to auditing changes
- track `Read`, `Removed` and `Modified` instances
- full `delta` is provided using the `Json Patch` Specification
- `Customise` what you write to the audit log with your own `Parser`
- Log anywhere, `File`, `Console` or bring bring your own if you need
- changes can be audited as `explicit` or `observed`
- capture who with the `IPrincipal` or `IClaimsPrincipal`
- Supports tracing, using the `OpenTelemerty/W3C` specification

## How does this differ from logging?

the following is a typical picture, but the real answer is: It depends (on your requirements)


|                                                                    | Auditing                                                            | Logging                                                                                  |
|--------------------------------------------------------------------|---------------------------------------------------------------------|------------------------------------------------------------------------------------------|
| Use                                                                | Proof of a business action, with who did it, against what and when. | To understand what is happening with a service, and ensure it can be keep up and running |
| Audience                                                           | External Auditors / Business                                        | DevOps Engineer                                                                          |
| Fails                                                              | Loudly (excpetions are thrown)                                      | Sliently (exceptions are swallowed)                                                      |
| Process style                                                      | Sync                                                                | Async                                                                                    |
| (typically) Stored for<br>(really depends on your<br>requirements) | possibly years (depending on your regulators and law)               | possibly weeks to months (depending on your requirements)                                |



# Code teaser

Lets take ASPNET Core, please consider using the OpenTelemetry package to get all the data.

## 1. Builder

```
var builder = Host
    .CreateDefaultBuilder()
    .ConfigureAuditable(conf =>
    {
        conf.Use<AspNet>();     //this is registering the ASPNET dependencies
        conf.UseWriter<File>(); //note the default writer is console
    })
    .ConfigureWebHostDefaults(x =>
    {
        x.UseStartup<TStartup>().UseTestServer();
    });
```

## 2. AspNET startup

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    //ensure its after auth*
    app.UseAuditable();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

}
```

## 3. add some auditable logs

```
[Route("/Account")]
[Authorize]
public class AccountController :  Controller
{
    private readonly IDocumentSession _session;
    private readonly IAuditable _auditable;
    private readonly ILogger<TestController> _logger;

    public TestController(
        IDocumentSession session,
        IAuditable auditable,
        ILogger<TestController> logger)
    {
        _session = session;
        _auditable = auditable;
        _logger = logger;
    }


    [HttpPut]
    public async Task<ActionResult> Put(AccountResource updatedAccount)
    {
        if (account == null) throw new ArgumentNullException(nameof(updatedAccount));
        await using var auditContext = _auditable.CreateContext("Account.Update");

        var account = await _session.GetById(updatedAccount.Id);
        auditContext.WatchTargets(account); //watch for detla's

        //modify
        account.Name = updatedAccount.Name;

        _logger.LogInformation("log out system information as normal");
        return new OkResult();
        
        //as we are within a using block, this will write the auditable entry
        //at this point.
    }
}
```

## output app-dir\1980-01-02-10-03-15_audit-id.auditable

a file is created for this audit entry (without line breaks)

```
{
    "Action": "Account.Update",
    "DateTime": "1980-01-02T10:03:15Z",
    "Initiator": {
        "Id": "abc-123",
        "Name": "dave"
    },
    "Environment": {
        "Host": "LAPTOP-VRDBEDO2",
        "Application": "testhost.x86, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    },
    "Request": {
        "SpanId": "16ba66e3222c3149",
        "TraceId": "4bf92f3577b34da6a3ce929d0e0e4736",
        "ParentId": "00f067aa0ba902b7"
    },
    "Targets": [
        {
            "Type": "Auditable.AspNetCore.Tests.Account",
            "Id": "2",
            "Delta": {
                "Name": [
                    "Dave",
                    "Chan"
                ]
            },
            "Style": "Observed",
            "Audit": "Modified"
        }
    ],
    "Id": "audit-id"
}
```
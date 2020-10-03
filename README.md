# auditable

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/7dc1cf53814a4165b45306b158bf06bc)](https://app.codacy.com/gh/dbones-labs/auditable?utm_source=github.com&utm_medium=referral&utm_content=dbones-labs/auditable&utm_campaign=Badge_Grade_Settings)

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

imagine you want to load an account instance from the db and update it.

```
[Route("/Account")]
[Authorize]
public class AccountController : Controller
{
    private readonly DocumentSession _session;
    private readonly IAuditable _auditable;
    private readonly ILogger<TestController> _logger;

    public AccountController(
        DocumentSession session,
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
        if (updatedAccount == null) throw new ArgumentNullException(nameof(updatedAccount));
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

there is some keys bits of information:

- Initiator - is the person making the change (note this should support OAuth2/OIDC)
- Environment - is the server that is running the app, and what version of the app
- Request - is information about the single request (pulling the w3c info using OpenTelemetry)
- Targets - all the objects being observed to see if they were Read, Modified or Deleted
- Id - is the unique id of the Auditable entry

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

# Why call it Auditable

All the information we are capturing is to support an Audit from an Auditor (typically external). They will look over the Auditable Entries during their Audit. Once the External Audit of an Application is complete, they will produce an Audit Report.

So by using this your application is `Auditable`.
---
title: ASPNET Example
parent: Quick Examples
has_children: false
nav_order: 2
---

# Simple ASPNET example

This is how you can add `auditable` to you ASPNET application with 

:pencil2: we has setup

- OpenTelemetry - to provide request infromation
- OAuth2/ODIC - to show who was the initator of the call
 

after this you will have 

- Auditable enabled
- Writing Audit logs to File

## 1. Builder

```csharp
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

## 2. ASPNET Core startup

```csharp
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

```csharp
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

```json
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

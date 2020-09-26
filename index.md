---
title: auditable
has_children: false
nav_order: 1
---

# Welcome to `auditable`

This is a small auditing framework

The need for auditing already exists in projects, the requirement to log information to prove something happened, but also to capture who, what and when a business action happened.

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

# Why call it Auditable

All the information we are capturing is to support an Audit from an Auditor (typically external). They will look over the Auditable Entries during their Audit. Once the External Audit of an Application is complete, they will produce an Audit Report.

So by using this your application is `Auditable`.
# auditable

This is a small auditing framework

The need for auditing already exists in projects, the requirement to log information to prove something happened, but also to capture who, what and when a business action happened.

## Features

- `Unit of work style` to auditing changes
- track `Read`, `Removed` and `Modified` instance
- full `delta` is provided using the `Json Patch` Specification
- `Customise` what you write to the audit log with your own `Parser`
- Log anywhere, `File`, `Console` or bring bring your own if you need
- changes can be audited as `explicit` or `observed`
- capture who with the `IPrincipal` or `IClaimsPrincipal`
- Supports tracing, using the `OpenTelemerty/W3C` specification


# A teaser of code

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
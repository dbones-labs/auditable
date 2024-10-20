---
title: Creating Context
parent: Features
has_children: false
nav_order: 1
---

# Context

All Auditng happens withing a context.

It has

- a Name
- the collection of targets being observed


## Create

Before you do anything you need to create a context, you can do this by injecting `Auditable` into your class, then in the method you can create a context using the `_auditable.CreateContext("name.of.context");`


## 2 ways to write to the log

### via `using` ✔️

Recommended way, this will automatically write on the successful completion of tasks (within the `IDispose` scope)

On exception, no auditable logs are written.

### explicit `write()`

if you want to have full control then you can call `write()` yourself.

## Example Code

The example below shows how to inject and use the contect within a `using` scope

- 1️⃣ inject
- 2️⃣ create contect, with a meaningful name
- 3️⃣ add targets to the context, and do work
- 4️⃣ end of the `using` scope, this is when the auditable log will be evaluated and written

```csharp
[Route("/test")]
[Authorize]
public class TestController :  Controller
{
    private readonly IAuditable _auditable;
    private readonly ILogger<TestController> _logger;

    public TestController(
        IAuditable auditable, // 1️⃣
        ILogger<TestController> logger)
    {
        _auditable = auditable;
        _logger = logger;
    }


    [HttpGet]
    public async Task<ActionResult> Get()
    {
        await using var auditContext = _auditable.CreateContext("test.get"); // 2️⃣ 
        
        auditContext.Read<Person>("123"); // 3️⃣
        _logger.LogInformation("called the get method, and did some awesome things");
        
        // 4️⃣
        return new OkResult();
    }
}
```

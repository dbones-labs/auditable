---
title: Marking as Removed
parent: Features
has_children: false
nav_order: 3
---

# Marking as Removed

We can record a target as being removed

You will need to have a `AuditableContext`, check the docs for Creating Context.

## Explicitly removing the target via its ID.

Only your code will know if the target was removed, so in this case we can easily let `auditable` know

- 1️⃣ tell auditable that this object was removed.

```csharp 
await using var auditContext = _auditable.CreateContext("Person.Read");
auditContext.Removed<Person>("123"); // 1️⃣
```

This will write the following in the Auditable Log

- 1️⃣ The entry be `Explicit`
- 2️⃣ `Audit` of the `Target` will be set to `Removed`

```json
"Targets": [
    {
        "Type": "Auditable.AspNetCore.Tests.Person",
        "Id": "123",
        "Delta": null,
        "Style": "Explicit", // 1️⃣
        "Audit": "Removed" // 2️⃣ 
    }
]
```

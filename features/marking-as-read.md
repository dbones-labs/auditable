---
title: Marking as Read
parent: Features
has_children: false
nav_order: 2
---

# Marking as Read

Proving that someone has accessed a Target can be very important, so lets see how we cna do that.

you will need to have a `AuditableContext`, check the docs for Creating Context.


## 2 ways to add a target as being READ.

### Adding the instance as a target

- 1️⃣ Read the object
- 2️⃣ Add it to the auditing context

```csharp
var person = _documentSession.GetById("123"); // 1️⃣
await using var auditContext = _auditable.CreateContext("Person.Read");
auditContext.WatchTargets(person); // 2️⃣ 
```

This will write the following in the Auditable Log

- 1️⃣ The entry be `Observed`, `auditable` will confirm that the state of person did not change
- 2️⃣ `Audit` of the `Target` will be set to `Read`

```json
"Targets": [
    {
        "Type": "Auditable.AspNetCore.Tests.Person",
        "Id": "123",
        "Delta": null,
        "Style": "Observed", // 1️⃣
        "Audit": "Read" // 2️⃣ 
    }
]
```



### Explicitly adding the target via its ID.

if you know that the target was only read, then use this to tell this information to `auditable`

- 1️⃣ tell auditable that this object was read.

```csharp 
await using var auditContext = _auditable.CreateContext("Person.Read");
auditContext.Read<Person>("123"); // 1️⃣
```

This will write the following in the Auditable Log

- 1️⃣ The entry be `Explicit`, `auditable` did not have to figure it out
- 2️⃣ `Audit` of the `Target` will be set to `Read`

```json
"Targets": [
    {
        "Type": "Auditable.AspNetCore.Tests.Person",
        "Id": "123",
        "Delta": null,
        "Style": "Explicit", // 1️⃣
        "Audit": "Read" // 2️⃣ 
    }
]
```

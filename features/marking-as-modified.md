---
title: Marking as Modified
parent: Features
has_children: false
nav_order: 4
---

# Marking as Modified

This is where a `auditable` will compare any taget which has its instance registed, and look for all the delta's

you will need to have a `AuditableContext`, check the docs for Creating Context.


## 2 ways to add the target to be observed as modified

### via `CreateContext`

- 1️⃣ Read the object
- 2️⃣ Add it to the auditing context (you can add multiple targets here)
- 3️⃣ the target is modified
- 4️⃣ at the time of writing `auditable` will compare state to idenity these changes

```csharp
var person = _documentSession.GetById("123"); // 1️⃣
await using var auditContext = _auditable.CreateContext("Person.Read", person); // 2️⃣

person.Name = "Chan"; // 3️⃣

// 4️⃣
```

### via `CreateContext`


- 1️⃣ Read the object
- 2️⃣ Add it to the auditing context (you can add multiple targets or call it many times for the different targets)
- 3️⃣ the target is modified
- 4️⃣ at the time of writing `auditable` will compare state to idenity these changes

```csharp
var person = _documentSession.GetById("123"); // 1️⃣
await using var auditContext = _auditable.CreateContext("Person.Read", person);
auditContext.WatchTargets(person); // 2️⃣

person.Name = "Chan"; // 3️⃣

// 4️⃣
```

## Output in the Autiable Log

Both ways above will yield the same output

- 1️⃣ `Delta` will be populated with all the differences, using the `JSONP` spec 
- 2️⃣ The entry be `Observed`, `auditable` compares the state from the when the oject was added vs the state at Writing the auditable log
- 3️⃣ `Audit` of the `Target` will be set to `Modified`

```json
"Targets": [
    {
        "Type": "Auditable.AspNetCore.Tests.Person",
        "Id": "2",
        "Delta": { // 1️⃣
            "Name": [
                "Dave",
                "Chan"
            ]
        },
        "Style": "Observed",  // 2️⃣
        "Audit": "Modified" // 3️⃣
    }
]
```

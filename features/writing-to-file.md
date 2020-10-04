---
title: Writing to file
parent: Features
has_children: false
nav_order: 11
---

# Writing to File

Writing auditable entries using the `File` writer. This will

- write 1 file per audit entry
- allow the application to set the audit directory and file name
- will throw an exception if the file cannot be written

## When to use

If you have no database or any thing else which can act as the primary Unit-of-work

## Setup

Writers are setup during the Application HostBuilder.


- 1️⃣ Register the File Writer
- 2️⃣ Example of settings the file name.


```csharp
var builder = Host
    .CreateDefaultBuilder()
    .ConfigureAuditable(conf =>
    {
        conf.UseWriter<File>()    // 1️⃣
            .Setup(options=> 
            {
                options.GetFileName = (id, action) =>  // 2️⃣
                {
                    Code.Require(()=> !string.IsNullOrEmpty(id), nameof(id));
                    var date = SystemDateTime.UtcNow.ToString("yyyy-MM-dd-H-mm-ss");
                    return $"{date}_{id}.auditable";
                }
            });
    });
```
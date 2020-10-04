---
title: Initiator via Thread
parent: Features
has_children: false
nav_order: 20
---

# Initiator Thread Principal

To capture the current user against each log entry, `auditable` by default looks for the `Thread.CurrentPrincipal`


## When to use

If you have set Thread Principal, however it can be easily extracted.

note that this is not recommended:

https://docs.microsoft.com/en-us/aspnet/core/migration/claimsprincipal-current?view=aspnetcore-3.1

## Setup

ensure that your code sets the Principal at the beginning of the IoC container `Scope`.
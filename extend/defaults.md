---
title: Defaults
parent: Extend
has_children: false
nav_order: 2
---

# Defaults

`auditable` has many area's of where it can be overridden. For most of these there is a default provider.

The key part to understand is that `auditable` makes use of the IoC container (using the dotnet core abstractions)

This means you can easily override any part of auditable.


## Things you may want to override

### Collectors

we have several collectors which augment the audit entry with extra information

- Environment - host and application instance
- Initiator - User which the request is running under
- EntityId - Gets the ID of the object root
- Request - trace id

### Parser

Constructs the audit entry into a serialized string, if you need a custom log message then look into this area

note that the Parser makes most use of the collectors.

### Writer

where the log entry is written to, the default is console (which is for testing and getting started, please use another)

## How to override

there are a few ways


### Create a Extension ✔️

its probably best to look directly at the code for the ASPNET implementation

https://github.com/dbones-labs/auditable/blob/master/src/Auditable.AspNetCore/AspNet.cs

It is a quick way to access the container, but looks a bit more natural to an end consumer.

### Register your overriding type last

A more manual method, consider if you want to add your own way of collecting the Environment, just implement the interface and register after you have registered all of the `auditable` components


`auditable` will then make use of your implementation.

## Provided Extensions

- ASPNET
- File Writer

where possible we will document them in the features section if they are owned by us.

---
title: Initiator via Claims
parent: Features
has_children: false
nav_order: 21
---

# Initiator Claims Principal

To capture the current user against each log entry, `auditable` can also support the Claims Principal.


## When to use ✔️

This is the recommended way to grab the user

## Setup

### of `auditable` 

Please follow the Quick Example, this shows how to setup `auditable` fully

`auditable`'s implementation grabs the name and the id using the following Claims

```csharp
initiatorCollector.Initiator = new Initiator
{
    Id =  user.FindFirstValue(ClaimTypes.NameIdentifier), 
    Name = user.FindFirstValue(ClaimTypes.Name) 
};
```

https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.claims.claimtypes?view=netframework-4.8

### of your OAuth Provider

Consider that you may have to Map from the OAuth Claim from your provider as follows:

- 1️⃣ id mapped to the `NameIdentifier`
- 2️⃣ name mapped to the `Name`


```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GitHub";
})
.AddCookie()
.AddOAuth("GitHub", options =>
{
    //..

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id"); // 1️⃣
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name"); // 2️⃣
    options.ClaimActions.MapJsonKey("urn:github:login", "login");
    
    //..
});
```

A full example can be found here: https://www.jerriepelser.com/blog/authenticate-oauth-aspnet-core-2/
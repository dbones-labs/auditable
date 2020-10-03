# auditable

[![release](https://img.shields.io/github/v/release/dbones-labs/auditable?logo=nuget)](https://github.com/dbones-labs/auditable/releases) [![Nuget](https://img.shields.io/badge/nuget-auditable-blue)](https://github.com/orgs/dbones-labs/packages?repo_name=auditable)
[![docs](https://img.shields.io/badge/docs-auditable-blue)](https://dbones-labs.github.io/auditable/)

[![dbones-labs](https://circleci.com/gh/dbones-labs/auditable.svg?style=shield)](https://app.circleci.com/pipelines/github/dbones-labs/auditable) 
[![codecov](https://codecov.io/gh/dbones-labs/auditable/branch/master/graph/badge.svg?token=0AE8TL5PR3)](undefined) 
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/efd93328aebe4815a5710df7bbce5d03)](https://www.codacy.com/gh/dbones-labs/auditable/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=dbones-labs/auditable&amp;utm_campaign=Badge_Grade) 


This is a small auditing framework

The need for auditing already exists in projects, the requirement to log information to prove something happened, but also to capture who, what and when a business action happened.

## Features

- `Unit of work style` to auditing changes
- track `Read`, `Removed` and `Modified` instances
- full `delta` is provided using the `Json Patch` Specification
- `Customise` what you write to the audit log with your own `Parser`
- Log anywhere, `File`, `Console` or bring bring your own if you need
- changes can be audited as `explicit` or `observed`
- capture who with the `IPrincipal` or `IClaimsPrincipal`
- Supports tracing, using the `OpenTelemerty/W3C` specification

# Downloads

you can find all packages here:

[![Nuget](https://img.shields.io/badge/nuget-auditable-blue)](https://github.com/orgs/dbones-labs/packages?repo_name=auditable)


## Major releases

[![Nuget](https://img.shields.io/github/v/release/dbones-labs/auditable?logo=nuget)](https://github.com/dbones-labs/auditable/releases)

We use Milestones to represent an notable release


## Patch / feature releases

We use a variant of Githubflow, so all feature branches have their own pre-release packages



# Docs and examples

check out our docs for examples and more information

[![docs](https://img.shields.io/badge/docs-auditable-blue)](https://dbones-labs.github.io/auditable/)

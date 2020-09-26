---
title: Auditing vs Debugging
has_children: false
nav_order: 2
---

# How does this differ from logging?

the following is a typical picture, but the real answer is: It depends (on your requirements)


|                                                                    | Auditing                                                            | Logging                                                                                  |
|--------------------------------------------------------------------|---------------------------------------------------------------------|------------------------------------------------------------------------------------------|
| Use                                                                | Proof of a business action, with who did it, against what and when. | To understand what is happening with a service, and ensure it can be keep up and running |
| Audience                                                           | External Auditors / Business                                        | DevOps Engineer                                                                          |
| Fails                                                              | Loudly (excpetions are thrown)                                      | Sliently (exceptions are swallowed)                                                      |
| Process style                                                      | Sync                                                                | Async                                                                                    |
| (typically) Stored for<br>(really depends on your<br>requirements) | possibly years (depending on your regulators and law)               | possibly weeks to months (depending on your requirements)                                |


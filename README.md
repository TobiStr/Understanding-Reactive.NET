# Understanding-Reactive.NET

This repository is a showcase on how to correctly use Reactive Extensions in .NET.

To this date I have not found a detailed source on how to correctly use all those Reactive Extensions in .NET.
I've found myself many times making mistakes or misunderstanding the behavior of some Reactive Extensions, so i decided to create this repository and create tests and benchmarks on every extension there is.

## Package References

The first problem i came across, was always what package i have to import to use specific functions.

If you just want to handle Observables, then the Package "System.Reactive" is the one you are looking for.
Sometimes you might want to handle Async Enumerables, here "System.Interactive.Linq" comes in handy. And if you like, you can also use some Enumerable Extensions under "System.Interactive".

Concluded:

| Package                  | Contains                                   |
| ------------------------ | ------------------------------------------ |
| System.Reactive          | Observable Class and Observable Extensions |
| System.Interactive       | Enumerable Extensions                      |
| System.Interactive.Async | Async Enumerable Extensions                |

(For older packages than 5.0.0 this information might not be correct)

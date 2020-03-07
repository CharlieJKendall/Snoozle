# Snoozle

The Snoozle framework removes the need for boilerplate code whilst providing fully-fledged REST APIs via simple model-based resource definitions. Behind the scenes, Snoozle uses the .NET expression tree API to dynamically write and compile highly optimised code based on your simple model definitions.

!!! info "Supported .NET Versions"
    - .NET Core 2.1
    - .NET Core 2.2
    - .NET Core 3.0
    - .NET Core 3.1

Snoozle aims to make the creation of fully customisable REST APIs accessible to software developers of all levels. Example use cases for Snoozle REST APIs are:

- Easily provide data persistence for a SPA, with minimal knowledge of C#
- Expose data in a structured way without the need for the maintenance overheads of writing your own REST API code components
- Mock out existing REST APIs for testing purposes
- Provide a strong interface to remove the dependency of an application's back-end from the front-end, enabling concurrent development on both sides

# Example

Setup and configuration of a Snoozle REST API is simple.

1. Define your data model, and implement the `IRestResource` marker interface

``` cs
public class Cat : IRestResource
{
    public int? Id { get; set; }

    public int HairLength { get; set; }

    public string Name { get; set; }
}
```

2. Create your resource configuration class that inherits from the abstract configuration builder

``` cs
public class CatResourceConfiguration : SqlResourceConfigurationBuilder<Cat>
{
    public override void Configure()
    {
        ConfigurationForProperty(x => x.HairLength).HasColumnName("HairLengthInMeters");
        ConfigurationForProperty(x => x.Id).HasColumnName("CatId").IsPrimaryIdentifier();
    }
}

```
# Getting Started

This section outlines how to go from nothing to having a Snoozle-based REST API up and running. This section assumes the following:

- A basic knowledge of C# and Visual Studio
- A compatible version of .NET installed (download the SDK from [here](https://dotnet.microsoft.com/download))
- Visual Studio installed (download the Community Edition for free [here](https://visualstudio.microsoft.com/downloads/))
- [Postman](https://www.postman.com/downloads/) (or equivalent) for testing the endpoints

### Choose your data provider

Snoozle exposes a set of base classes and interfaces that can be implemented to provide data persistence using a variety of technologies. The basic Snoozle package provides in-memory persistence, which is what this demo will use.

See the [data providers](data-providers.md) page for a full list.

### Create a new web project in Visual Studio

For this guide, we will be using .NET Core 3.0 and Visual Studio Community Edition 2019.

- Open Visual Studio
- Select 'Create a new project'
- Search for 'core web application' in the template search bar
- Select the 'ASP.NET Core Web Application' template from the list and create the new web project
- Select the 'API' option and leave the rest of the default settings as they are

You should now have a bare-bones .NET Core 3.0 API web application- you can run it using the green play button in the toolbar at the top.

### Install Snoozle and add it to your application Startup.cs

Install Snoozle via the NuGet package manager console. This can be found via the top navigation menu Tools > NuGet Package Manager > Package Manager Console.

Install the latest version of Snoozle using the Package Manager Console by running `Install-Package Snoozle`. Once this has succeeded, add a using directive for Snoozle.InMemory to the Startup.cs file `using Snoozle.InMemory;`.

This will enable you to call the `.AddSnoozleInMemory()` extension method from any method returning an `IMvcBuilder` object, for example `servivces.AddControllers().AddSnoozleInMemory();`.

### Create a resource model

Create a new model class named appropriately for the resource you want to model. In this case, we're going to be modelling a cat.

Ensure that the class is public and inherits from the `IRestResource` interface which is provided in the root Snoozle namespace. Add any required properties for your model. This should include one property that is representative of the primary key identifier.

``` cs
public class Cat : IRestResource
{
    public Guid? Id { get; set; }

    public int? HairLength { get; set; }

    public string Name { get; set; }
}
```

### Create a resource configuration 

Create a new class (either in a new file, or in the same file as the resource) for the resource configuration, e.g. `CatResourceConfiguration`. This must inherit from the base resource configuration type, which varies for each data provider.

For the in-memory data provider, this is called `InMemoryResourceConfigurationBuilder<>`. Implement this and then override the `Configure()` method.

Primary key properties are detected by convention if they are called `Id` or `<resource_name>Id` (e.g. `CatId`), however we will explicitly configure it here as an example. Configurations for the resource can be set at either the property- or model-level using the `ConfigurationForProperty()` and `ConfigurationForModel()` methods.

``` cs
public class CatResourceConfiguration : InMemoryResourceConfigurationBuilder<Cat>
{
    public override void Configure()
    {
        ConfigurationForProperty(x => x.Id).IsPrimaryIdentifier();
    }
}
```

### Run the application and test with Postman

We're now ready to run the application and see it in action. Run the web application, and then form a POST request to the following endpoint with the `Content-Type: application/json` header.

Endpoint: `<base_url>/api/cats`, for example `https://localhost:44358/api/cats` with the following request body:

``` json
{
    "name": "Fuzzles",
    "hairLength": 15
}
```

You will then be able to make a GET request to the same endpoint, which will return you the stored data along with each ID value.

For further examples and guides, see the Walkthroughs section of these docs or visit the https://github.com/CharlieJKendall/SnoozleDemos repository.
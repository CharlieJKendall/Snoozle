Model configuration is applied at the REST resource (model) level. This page details core configuration, however different data providers may add extra model configuration values.

These are configured via the `IModelConfigurationBuilder` in the `Configure()` method of a resource configuration:

``` cs
public override void Configure()
{
    ConfigurationForModel()
        .HasRoute("new-api-route")
        .HasAllowedHttpVerbs(HttpVerbs.Put | HttpVerbs.Post);
}
```

#### HasRoute

Default: The name of the resource with the character `s` appended

Sets the route that the resource can be accessed at. For example, `HasRoute("another-route")` would expose the resource at `https://baseuri/api/another-route`.

#### HasAllowedHttpVerbs

Default: The same as the value set in the global configuration

The HTTP verbs that every resource allows. This overrides the globally set value.
Property configuration is applied at the REST resource property level. This page details core configuratoin, however different data providers may add extra property configuration values.

These are configured via the `IPropertyConfigurationBuilder` in the `Configure()` method of a resource configuration:

``` cs
public override void Configure()
{
    ConfigurationForProperty(x => x.Id)
        .HasComputedValue(HttpVerbs.POST).RandomlyGeneratedGuid()
        .IsPrimaryIdentifier();
}
```

#### IsReadOnly

Default: False

Whether the data is returned on `GET` requests or not. This is generally used for metadata such as timestamps or trace identifiers.

#### IsPrimaryIdentifier

Default: False unless convention criteria are satisfied

!!! tip "Convention"
    A property with the name `Id` or `<resource_class_name>Id` will be set as the primary identifier by convention if no other property is explicitly set.

Defines which property defines the identity of a resource. This will be used to access specific resources in the URL.

#### HasComputedValue

Specifies that the value of the property is automatically computed and not defined in the payload of the HTTP request. This method returns a builder object that is used to define the value of this property:

**DateTimeNow**

Sets the value of the property to the value of `DateTime.Now` at request-time.

**DateTimeUtcNow**

Sets the value of the property to the value of `DateTime.UtcNow` at request-time.

**Custom**

Sets the value of the property to that specified by the `Func<TProperty>`. For example, the following would set the value to a random GUID for a property with type `Guid` on POST requests:

``` cs
ConfigurationForProperty(x => x.Id).HasComputedValue(HttpVerbs.POST).Custom(x => Guid.NewGuid());
```
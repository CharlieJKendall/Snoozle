Global configuration is applied at the application level. This page details core configuratoin, however different data providers may add extra global configuration values.

**Configuring in code**

Global configuration can be set in code from the relevant `AddSnoozle()` method in the `Startup.cs` file. This is generally used only for development convenience- production applications should define configuration in a separate configuration file (see below).

``` cs
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        .AddSnoozleInMemory(options =>
        {
            options.AllowedVerbs = HttpVerbs.All;
        });
}
```

**Configuring with appSettings.json**

Global configuration can be set in the appSettings.json file under a 'Snoozle' root entry:

``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Snoozle": {
    "AllowedVerbs": "post, get, put, delete"
  }
}

```

This is then passed to the relevent `AddSnoozle()` method in the `Startup.cs` file.

``` cs
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        .AddSnoozleInMemory(Configuration.GetSection("Snoozle"))
}
```

#### AllowedVerbs

Default: `HttpVerbs.All`

The HTTP verbs that every resource allows. This can be overridden at the resource-level if required.
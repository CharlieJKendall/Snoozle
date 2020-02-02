namespace Snoozle.Abstractions
{
    public interface IModelConfiguration
    {
        string Route { get; set; }

        HttpVerb AllowedVerbsFlags { get; set; }
    }
}
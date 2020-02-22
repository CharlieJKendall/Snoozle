namespace Snoozle.Abstractions
{
    public interface IModelConfiguration
    {
        string Route { get; set; }

        HttpVerbs AllowedVerbsFlags { get; set; }
    }
}
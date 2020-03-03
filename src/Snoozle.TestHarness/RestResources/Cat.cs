using System;

namespace Snoozle.InMemory.TestHarness.RestResources
{
    public class Cat : IRestResource
    {
        public Guid? Id { get; set; }

        public int? HairLength { get; set; }

        public string Name { get; set; }
    }

    public class CatResourceConfiguration : InMemoryResourceConfigurationBuilder<Cat>
    {
        public override void Configure()
        {
            ConfigurationForModel().HasJsonFilePath("C:/temp/cats.json");
            ConfigurationForProperty(x => x.Id).HasComputedValue(HttpVerbs.POST).RandomlyGeneratedGuid();
        }
    }
}

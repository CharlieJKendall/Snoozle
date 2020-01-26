using Snoozle.Core;
using Snoozle.Enums;
using Snoozle.RestResourceConfiguration;
using System;

namespace Snoozle.TestHarness.RestResources
{
    public class Cat : IRestResource
    {
        public int Id { get; set; }

        public int HairLength { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public long? UnconfiguredProperty { get; set; }
    }

    public class CatResourceConfiguration : AbstractResourceConfigurationBuilder<Cat>
    {
        public override void Configure()
        {
            ConfigurationForResource().HasTableName("Cats").HasAllowedHttpVerbs(HttpVerb.GET | HttpVerb.POST | HttpVerb.PUT).HasRoute("cattyboys");

            ConfigurationForProperty(x => x.HairLength).HasColumnName("HairLengthInMeters");
            ConfigurationForProperty(x => x.Id).HasColumnName("CatId").IsPrimaryIdentifier();
            ConfigurationForProperty(x => x.DateCreated).HasComputedColumnValue().DateTimeNow();
            ConfigurationForProperty(x => x.Name).HasColumnName("WrongColumnName");
            ConfigurationForProperty(x => x.Name).HasColumnName("CatName").HasComputedColumnValue().Custom(() => "HELLO");
            ConfigurationForProperty(x => DateTime.Now);
        }
    }
}

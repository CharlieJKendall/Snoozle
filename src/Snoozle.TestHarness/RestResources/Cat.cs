﻿using Snoozle.Abstractions;
using Snoozle.Enums;
using Snoozle.Extensions;
using Snoozle.SqlServer.Implementation;
using Snoozle.SqlServer.Extensions;
using System;

namespace Snoozle.TestHarness.RestResources
{
    public class Cat : IRestResource
    {
        public int Id { get; set; }

        public Test? HairLength { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public long? UnconfiguredProperty { get; set; }
    }

    public class CatResourceConfiguration : SqlResourceConfigurationBuilder<Cat>
    {
        public override void Configure()
        {
            ConfigurationForModel().HasTableName("Cats").HasAllowedHttpVerbs(HttpVerb.All).HasRoute("cattyboys");

            ConfigurationForProperty(x => x.HairLength).HasColumnName("HairLengthInMeters");
            ConfigurationForProperty(x => x.Id).HasColumnName("CatId").IsPrimaryIdentifier();
            ConfigurationForProperty(x => x.DateCreated).HasComputedValue().DateTimeNow();
            ConfigurationForProperty(x => x.Name).HasColumnName("WrongColumnName");
            ConfigurationForProperty(x => x.Name).HasColumnName("CatName").HasComputedValue().Custom(() => "HELLO");
        }
    }

    public enum Test
    {
        HEY = 1
    }

}

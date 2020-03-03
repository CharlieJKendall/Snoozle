﻿using Snoozle.Abstractions;
using Snoozle.Exceptions;
using Snoozle.InMemory.Implementation;
using System.Linq;

namespace Snoozle.InMemory
{
    public abstract class InMemoryResourceConfigurationBuilder<TResource> : BaseResourceConfigurationBuilder<TResource, IInMemoryPropertyConfiguration, IInMemoryResourceConfiguration, IInMemoryModelConfiguration>
        where TResource : class, IRestResource
    {
        protected override IPropertyConfigurationBuilder<TProperty, IInMemoryPropertyConfiguration> CreatePropertyConfigurationBuilder<TProperty>(
            IInMemoryPropertyConfiguration propertyConfiguration)
        {
            return new InMemoryPropertyConfigurationBuilder<TResource, TProperty>(propertyConfiguration);
        }

        protected override IInMemoryResourceConfiguration CreateResourceConfiguration()
        {
            return new InMemoryResourceConfiguration<TResource>(ModelConfiguration, PropertyConfigurations);
        }

        protected override IInMemoryModelConfiguration CreateModelConfiguration()
        {
            return new InMemoryModelConfiguration<TResource>();
        }

        protected override IInMemoryPropertyConfiguration CreatePropertyConfiguration()
        {
            return new InMemoryPropertyConfiguration();
        }

        protected override IModelConfigurationBuilder<IInMemoryModelConfiguration> CreateModelConfigurationBuilder()
        {
            return new InMemoryModelConfigurationBuilder(ModelConfiguration);
        }

        protected override void SetPropertyConfigurationDefaults()
        {
            base.SetPropertyConfigurationDefaults();
        }

        protected override void ValidateFinal()
        {
            base.ValidateFinal();

            ExceptionHelper.InvalidOperation.ThrowIfTrue(
                PropertyConfigurations.Single(prop => prop.IsPrimaryResourceIdentifier).ValueComputationFunc == null,
                $"The primary identifier for {typeof(TResource).Name} must have a unique value computation function defined for it (e.g. () => Guid.NewGuid()).");
        }
    }
}

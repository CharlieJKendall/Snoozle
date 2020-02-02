using Snoozle.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snoozle.Abstractions
{
    public abstract class BaseResourceConfigurationBuilder<TResource, TPropertyConfiguration, TResourceConfiguration, TModelConfiguration> : IResourceConfigurationBuilder<TPropertyConfiguration, TResourceConfiguration, TModelConfiguration>
        where TResource : class, IRestResource
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TResourceConfiguration : class, IResourceConfiguration<TPropertyConfiguration, TModelConfiguration>
        where TModelConfiguration : class, IModelConfiguration
    {
        private readonly Dictionary<string, TPropertyConfiguration> _propertyConfigurations = new Dictionary<string, TPropertyConfiguration>();

        public IEnumerable<TPropertyConfiguration> PropertyConfigurations => _propertyConfigurations.Values;

        public TModelConfiguration ModelConfiguration { get; private set; }

        protected abstract TModelConfiguration CreateModelConfiguration();

        protected abstract IModelConfigurationBuilder<TModelConfiguration> CreateModelConfigurationBuilder();

        protected abstract TPropertyConfiguration CreatePropertyConfiguration();

        protected abstract IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> CreatePropertyConfigurationBuilder<TProperty>(TPropertyConfiguration propertyConfiguration);

        protected abstract TResourceConfiguration CreateResourceConfiguration();

        public abstract void Configure();

        public IModelConfigurationBuilder<TModelConfiguration> ConfigurationForModel()
        {
            return CreateModelConfigurationBuilder();
        }

        public IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> ConfigurationForProperty<TProperty>(Expression<Func<TResource, TProperty>> member)
        {
            var memberExpression = member?.Body as MemberExpression;

            ExceptionHelper.Argument.ThrowIfTrue(memberExpression == null, $"Parameter '{nameof(member)}' must be a property selection expression.", nameof(member));

            string propertyName = memberExpression?.Member?.Name;

            TPropertyConfiguration config = _propertyConfigurations.GetValueOrDefault(propertyName) as TPropertyConfiguration;

            ExceptionHelper.InvalidOperation.ThrowIfTrue(config == null, $"No configuration is defined for property: ${propertyName}");

            return CreatePropertyConfigurationBuilder<TProperty>(config);
        }

        public TResourceConfiguration BuildResourceConfiguration()
        {
            // Create default instances and apply necessary property values
            SetPropertyConfigurationDefaults();
            SetModelConfigurationDefaults();

            // Update any properties to have values changed depending on conventions, e.g. column named 'Id' is primary identifier
            SetConventionsForProperties();
            SetConventionsForModel();

            // Apply user-defined configuration to override the defaults and conventions if applicable
            Configure();

            // Carry out final validation checks after user configuration has been applied
            ValidateFinal();

            return CreateResourceConfiguration();
        }

        protected virtual void SetPropertyConfigurationDefaults()
        {
            PropertyInfo[] properties = typeof(TResource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            for (int i = 0; i < properties.Length; i++)
            {
                // Create a configuration instance for each property
                TPropertyConfiguration configuration = CreatePropertyConfiguration();
                PropertyInfo property = properties[i];

                // Set any defaults that can't be set using auto-property defaults
                configuration.Index = i;
                configuration.PropertyName = property.Name;
                configuration.PropertyType = property.PropertyType;

                _propertyConfigurations.TryAdd(property.Name, configuration);
            }
        }

        protected virtual void SetConventionsForProperties()
        {
            TPropertyConfiguration config;

            // Set column called [resource_type_name]Id to primary identifier (i.e. Person -> PersonId)
            var idPropName = typeof(TResource).Name.ToLowerInvariant() + "id";
            config = _propertyConfigurations.FirstOrDefault(prop => prop.Key.ToLower() == idPropName).Value;

            if (config != null)
            {
                config.IsPrimaryResourceIdentifier = true;
            }

            // Set column called Id to primary identifier
            config = _propertyConfigurations.FirstOrDefault(prop => prop.Key.ToLower() == "id").Value;

            if (config != null)
            {
                _propertyConfigurations.Values.ToList().ForEach(prop => prop.IsPrimaryResourceIdentifier = false);
                config.IsPrimaryResourceIdentifier = true;
            }
        }

        protected virtual void SetModelConfigurationDefaults()
        {
            ModelConfiguration = CreateModelConfiguration();
        }

        protected virtual void SetConventionsForModel()
        {
        }

        protected virtual void ValidateFinal()
        {
            ExceptionHelper.InvalidOperation.ThrowIfTrue(
                _propertyConfigurations.Values.Count(prop => prop.IsPrimaryResourceIdentifier) > 1,
                "There must be a maximum of 1 property marked as the primary identifier.");
        }
    }
}

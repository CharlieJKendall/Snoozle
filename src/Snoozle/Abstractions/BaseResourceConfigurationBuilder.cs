using Snoozle.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides methods for configuring a REST resource.
    /// </summary>
    public abstract class BaseResourceConfigurationBuilder<TResource, TPropertyConfiguration, TResourceConfiguration, TModelConfiguration> : IResourceConfigurationBuilder<TPropertyConfiguration, TResourceConfiguration, TModelConfiguration>
        where TResource : class, IRestResource
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TResourceConfiguration : class, IResourceConfiguration<TPropertyConfiguration, TModelConfiguration>
        where TModelConfiguration : class, IModelConfiguration
    {
        private readonly Dictionary<string, TPropertyConfiguration> _propertyConfigurations = new Dictionary<string, TPropertyConfiguration>();

        /// <summary>
        /// The property configurations for the resource.
        /// </summary>
        public IEnumerable<TPropertyConfiguration> PropertyConfigurations => _propertyConfigurations.Values;

        /// <summary>
        /// The model configuration for the resource.
        /// </summary>
        public TModelConfiguration ModelConfiguration { get; private set; }

        /// <summary>
        /// Override to create an instance of the custom model configuration.
        /// </summary>
        protected abstract TModelConfiguration CreateModelConfiguration();

        /// <summary>
        /// Override to create an instance of the custom model configuration builder.
        /// </summary>
        protected abstract IModelConfigurationBuilder<TModelConfiguration> CreateModelConfigurationBuilder();

        /// <summary>
        /// Override to create an instance of the custom property configuration.
        /// </summary>
        protected abstract TPropertyConfiguration CreatePropertyConfiguration();

        /// <summary>
        /// Override to create an instance of the custom property configuration builder.
        /// </summary>
        protected abstract IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> CreatePropertyConfigurationBuilder<TProperty>(TPropertyConfiguration propertyConfiguration);

        /// <summary>
        /// Override to create an instance of the custom resource configuration.
        /// </summary>
        protected abstract TResourceConfiguration CreateResourceConfiguration();

        /// <summary>
        /// Sets user-defined configuration for the REST resource.
        /// </summary>
        public abstract void Configure();

        /// <summary>
        /// Creates a <see cref="IModelConfigurationBuilder{TModelConfiguration}"/> for the current REST resource.
        /// </summary>
        public IModelConfigurationBuilder<TModelConfiguration> ConfigurationForModel()
        {
            return CreateModelConfigurationBuilder();
        }

        /// <summary>
        /// Creates a <see cref="IPropertyConfigurationBuilder{TProperty, TPropertyConfiguration}"/> for the current REST resource property.
        /// </summary>
        /// <param name="member">The property to configure.</param>
        public IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> ConfigurationForProperty<TProperty>(Expression<Func<TResource, TProperty>> member)
        {
            var memberExpression = member?.Body as MemberExpression;

            ExceptionHelper.Argument.ThrowIfTrue(memberExpression == null, $"Parameter '{nameof(member)}' must be a property selection expression.", nameof(member));

            string propertyName = memberExpression?.Member?.Name;

            TPropertyConfiguration config = _propertyConfigurations.GetValueOrDefault(propertyName) as TPropertyConfiguration;

            ExceptionHelper.InvalidOperation.ThrowIfTrue(config == null, $"No configuration is defined for property: ${propertyName}");

            return CreatePropertyConfigurationBuilder<TProperty>(config);
        }

        /// <summary>
        /// Builds a <see cref="TResourceConfiguration"/> object.
        /// </summary>
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

        /// <summary>
        /// Set any default values for the property configurations. This is called before the convention-based and user-defined configuration is applied.
        /// </summary>
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

        /// <summary>
        /// Set any values for the property configurations by conventions. This is called before the user-defined and after the default configuration is applied.
        /// </summary>
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

        /// <summary>
        /// Set any default values for the model configuration. This is called before the convention-based and user-defined configuration is applied.
        /// </summary>
        protected virtual void SetModelConfigurationDefaults()
        {
            ModelConfiguration = CreateModelConfiguration();
        }

        /// <summary>
        /// Set any values for the model configuration by conventions. This is called before the user-defined and after the default configuration is applied.
        /// </summary>
        protected virtual void SetConventionsForModel()
        {
        }

        /// <summary>
        /// Carry out final validation after all the property configurations (default, convention, user) have been applied.
        /// </summary>
        protected virtual void ValidateFinal()
        {
            ExceptionHelper.InvalidOperation.ThrowIfTrue(
                _propertyConfigurations.Values.Count(prop => prop.IsPrimaryResourceIdentifier) > 1,
                "There must be a maximum of 1 property marked as the primary identifier.");
        }
    }
}

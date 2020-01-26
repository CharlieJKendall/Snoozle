using Snoozle.Core;
using Snoozle.Enums;
using Snoozle.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Snoozle.RestResourceConfiguration
{
    public class ResourceModelConfiguration<T> : IResourceModelConfiguration<T>
        where T : class, IRestResource
    {
        private readonly Regex _tableNameRegex = new Regex(@"^[\p{L}_][\p{L}\p{N}@$#_]{0,127}$");
        private readonly Regex _routeRegex = new Regex(@"^[A-Za-z0-9]+$");

        public string TableName { get; private set; } = typeof(T).Name;

        public string Route { get; private set; } = typeof(T).Name + "s";

        public HttpVerb AllowedVerbsFlags { get; private set; } = HttpVerb.All;

        public IResourceModelConfiguration<T> HasTableName(string tableName)
        {
            ExceptionExtensions.ArgumentNull.ThrowIfNecessary(tableName, nameof(tableName));

            if (!_tableNameRegex.IsMatch(tableName))
            {
                throw new ArgumentException($"'{tableName}' is an invalid table name.");
            }

            TableName = tableName;
            return this;
        }

        public IResourceModelConfiguration<T> HasAllowedHttpVerbs(HttpVerb allowedVerbsFlags)
        {
            AllowedVerbsFlags = allowedVerbsFlags;
            return this;
        }

        public IResourceModelConfiguration<T> HasRoute(string route)
        {
            ExceptionExtensions.ArgumentNull.ThrowIfNecessary(route, nameof(route));

            if (!_routeRegex.IsMatch(route))
            {
                throw new ArgumentException($"'{route}' is an invalid route.");
            }

            Route = route;
            return this;
        }
    }

    public interface IResourceModelConfiguration<T>
        where T : class, IRestResource
    {
        string TableName { get; }

        string Route { get; }

        HttpVerb AllowedVerbsFlags { get; }

        IResourceModelConfiguration<T> HasTableName(string tableName);

        IResourceModelConfiguration<T> HasAllowedHttpVerbs(HttpVerb allowedVerbsFlag);

        IResourceModelConfiguration<T> HasRoute(string route);
    }

    public interface IPropertyConfigurationBuilderCore<TProperty>
    {
        IResourcePropertyConfiguration PropertyConfiguration { get; }
    }

    public interface IPropertyConfigurationBuilder<TProperty> : IPropertyConfigurationBuilderCore<TProperty>
    {
        IPropertyConfigurationBuilder<TProperty> IsReadOnlyColumn();

        IPropertyConfigurationBuilder<TProperty> HasSqlDbType(SqlDbType sqlDbType);

        IPropertyConfigurationBuilder<TProperty> HasColumnName(string columnName);

        IPropertyConfigurationBuilder<TProperty> IsPrimaryIdentifier();

        IComputedValueBuilder<TProperty> HasComputedColumnValue();
    }

    public interface IComputedValueBuilder<TProperty> : IPropertyConfigurationBuilderCore<TProperty>
    {
    }

    public class PropertyConfigurationBuilder<TResource, TProperty> : IPropertyConfigurationBuilder<TProperty>, IComputedValueBuilder<TProperty>
        where TResource : class, IRestResource
    {
        public IResourcePropertyConfiguration PropertyConfiguration { get; }

        public PropertyConfigurationBuilder(IResourcePropertyConfiguration propertyConfiguration)
        {
            PropertyConfiguration = propertyConfiguration;
        }

        public IPropertyConfigurationBuilder<TProperty> IsReadOnlyColumn()
        {
            PropertyConfiguration.IsReadOnly = true;
            return this;
        }

        public IPropertyConfigurationBuilder<TProperty> HasSqlDbType(SqlDbType sqlDbType)
        {
            PropertyConfiguration.SqlDbType = sqlDbType;
            return this;
        }

        public IPropertyConfigurationBuilder<TProperty> HasColumnName(string columnName)
        {
            PropertyConfiguration.ColumnName = columnName;
            return this;
        }

        public IPropertyConfigurationBuilder<TProperty> IsPrimaryIdentifier()
        {
            PropertyConfiguration.IsPrimaryResourceIdentifier = true;
            IsReadOnlyColumn();
            return this;
        }

        public IComputedValueBuilder<TProperty> HasComputedColumnValue()
        {
            return this;
        }
    }

    public static class ResourceConfigurationBuilderExtensions
    {
        public static IPropertyConfigurationBuilder<DateTime> DateTimeNow(this IComputedValueBuilder<DateTime> builder)
        {
            builder.PropertyConfiguration.ValueComputationFunc = () => DateTime.Now;
            return builder as IPropertyConfigurationBuilder<DateTime>;
        }

        public static IPropertyConfigurationBuilder<DateTime> DateTimeUtcNow(this IComputedValueBuilder<DateTime> builder)
        {
            builder.PropertyConfiguration.ValueComputationFunc = () => DateTime.UtcNow;
            return builder as IPropertyConfigurationBuilder<DateTime>;
        }

        public static IPropertyConfigurationBuilder<TProperty> Custom<TProperty>(this IComputedValueBuilder<TProperty> builder, Func<TProperty> computationFunc)
        {
            builder.PropertyConfiguration.ValueComputationFunc = ConvertToFuncObject(computationFunc);
            return builder as IPropertyConfigurationBuilder<TProperty>;
        }

        public static Func<object> ConvertToFuncObject<T>(Func<T> func)
        {
            return () => func();
        }
    }

    public abstract class AbstractResourceConfigurationBuilder<TResource> : IResourceConfigurationBuilder
        where TResource : class, IRestResource
    {
        private readonly ConcurrentDictionary<string, IResourcePropertyConfiguration> _propertyConfigurations = new ConcurrentDictionary<string, IResourcePropertyConfiguration>();

        private readonly ResourceModelConfiguration<TResource> _modelConfiguration = new ResourceModelConfiguration<TResource>();

        public IResourceModelConfiguration<TResource> ConfigurationForResource()
        {
            return _modelConfiguration;
        }

        public IPropertyConfigurationBuilder<TProperty> ConfigurationForProperty<TProperty>(Expression<Func<TResource, TProperty>> member)
        {
            if (!(member?.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException($"Parameter '{nameof(member)}' must be a property selection expression.", nameof(member));
            }

            string propertyName = memberExpression?.Member?.Name;
            IResourcePropertyConfiguration config = _propertyConfigurations.GetValueOrDefault(propertyName)
                ?? throw new InvalidOperationException($"No configuration is defined for property: ${propertyName}");

            return new PropertyConfigurationBuilder<TResource, TProperty>(config);
        }

        public abstract void Configure();

        internal void CreateAllPropertyConfigurationsWithDefaults()
        {
            List<PropertyInfo> properties = typeof(TResource).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            int index = 0;
            properties.ForEach(prop => _propertyConfigurations.TryAdd(prop.Name, new ResourcePropertyConfiguration(prop.Name, prop.PropertyType, index++)));
        }

        internal void SetConventionsForProperties()
        {
            IResourcePropertyConfiguration config;

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

        public IRuntimeResourceConfiguration BuildRuntimeConfiguration()
        {
            // Create a config instance for each property that has the default values
            CreateAllPropertyConfigurationsWithDefaults();

            // Update any properties to have values changed depending on conventions, e.g. column named 'Id' is primary identifier
            SetConventionsForProperties();

            // Apply user-defined configuration to override the defaults and conventions if applicable
            Configure();

            return new ResourceConfiguration<TResource>(_modelConfiguration, _propertyConfigurations.Select(prop => prop.Value));
        }
    }

    public interface IResourceConfigurationBuilder
    {
        IRuntimeResourceConfiguration BuildRuntimeConfiguration();
    }

    public class ResourceConfiguration<T> : IRuntimeResourceConfiguration
        where T : class, IRestResource
    {
        private readonly IEnumerable<IResourcePropertyConfiguration> _propertyConfigurations;

        private readonly IResourceModelConfiguration<T> _modelConfiguration;

        public ResourceConfiguration(IResourceModelConfiguration<T> modelConfiguration, IEnumerable<IResourcePropertyConfiguration> propertyConfigurations)
        {
            _modelConfiguration = modelConfiguration;
            _propertyConfigurations = propertyConfigurations;
        }

        public string TableName => _modelConfiguration.TableName;

        public IEnumerable<IResourcePropertyConfiguration> PropertyConfigurationsForRead => _propertyConfigurations.OrderBy(prop => prop.Index);

        public IResourcePropertyConfiguration PrimaryIdentifier => _propertyConfigurations.Single(prop => prop.IsPrimaryResourceIdentifier);

        public IEnumerable<IResourcePropertyConfiguration> PropertyConfigurationsForWrite => _propertyConfigurations.OrderBy(prop => prop.Index).Where(prop => !prop.IsReadOnly);

        public HttpVerb AllowedVerbsFlags => _modelConfiguration.AllowedVerbsFlags;

        public string Route => _modelConfiguration.Route;
    }

    public interface IRuntimeResourceConfiguration
    {
        string TableName { get; }

        HttpVerb AllowedVerbsFlags { get; }

        IEnumerable<IResourcePropertyConfiguration> PropertyConfigurationsForRead { get; }

        IResourcePropertyConfiguration PrimaryIdentifier { get; }

        IEnumerable<IResourcePropertyConfiguration> PropertyConfigurationsForWrite { get; }

        string Route { get; }
    }

    public interface IResourcePropertyConfiguration
    {
        string PropertyName { get; }

        Type PropertyType { get; }

        int Index { get; }

        SqlDbType? SqlDbType { get; set; }

        string ColumnName { get; set; }

        bool IsReadOnly { get; set; }

        bool IsPrimaryResourceIdentifier { get; set; }

        Func<object> ValueComputationFunc { get; set; }
    }

    public class ResourcePropertyConfiguration : IResourcePropertyConfiguration
    {
        public string PropertyName { get; }

        public Type PropertyType { get; }

        public int Index { get; }

        public SqlDbType? SqlDbType { get; set; }

        public string ColumnName { get; set; }

        public bool IsReadOnly { get; set; } = false;

        public bool IsPrimaryResourceIdentifier { get; set; } = false;

        public Func<object> ValueComputationFunc { get; set; }

        public ResourcePropertyConfiguration(string propertyName, Type propertyType, int index)
        {
            propertyType.TryUnwrapNullableType(out Type unwrappedType);

            PropertyName = propertyName;
            PropertyType = propertyType;
            ColumnName = propertyName;
            Index = index;
            SqlDbType = Maps.DefaultSqlDbTypeMap.GetValueOrDefault(unwrappedType);
        }
    }

    public static class Maps
    {
        public static ReadOnlyDictionary<Type, SqlDbType?> DefaultSqlDbTypeMap = new ReadOnlyDictionary<Type, SqlDbType?>(new Dictionary<Type, SqlDbType?>
        {
            { typeof(long), SqlDbType.BigInt },
            { typeof(byte[]), SqlDbType.VarBinary },
            { typeof(bool), SqlDbType.Bit },
            { typeof(string), SqlDbType.NVarChar },
            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
            { typeof(decimal), SqlDbType.Decimal },
            { typeof(double), SqlDbType.Float },
            { typeof(float), SqlDbType.Float },
            { typeof(int), SqlDbType.Int },
            { typeof(short), SqlDbType.SmallInt },
            { typeof(TimeSpan), SqlDbType.Time },
            { typeof(Guid), SqlDbType.UniqueIdentifier },
            { typeof(byte), SqlDbType.TinyInt }
        });
    }
}

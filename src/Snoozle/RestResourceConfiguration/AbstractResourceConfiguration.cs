using Snoozle.Core;
using Snoozle.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snoozle.RestResourceConfiguration
{
    public class ResourceModelConfiguration<T>
        where T : class, IRestResource
    {
        internal string TableName { get; private set; }

        public ResourceModelConfiguration<T> HasTableName(string tableName)
        {
            TableName = tableName;
            return this;
        }
    }

    public abstract class AbstractResourceConfigurationBuilder<T> : IResourceConfigurationBuilder<T>
        where T : class, IRestResource
    {
        private readonly ConcurrentDictionary<string, ResourcePropertyConfiguration> _propertyConfigurations = new ConcurrentDictionary<string, ResourcePropertyConfiguration>();

        private readonly ResourceModelConfiguration<T> _resourceConfiguration = new ResourceModelConfiguration<T>();

        public ResourceModelConfiguration<T> ConfigurationForResource()
        {
            return _resourceConfiguration;
        }

        public ResourcePropertyConfiguration ConfigurationForProperty<TProperty>(Expression<Func<T, TProperty>> member)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(member);
            return _propertyConfigurations.GetValueOrDefault(propertyInfo.Name);
        }

        private PropertyInfo GetPropertyInfo<TMember, TProperty>(Expression<Func<TMember, TProperty>> propertyExpression)
        {
            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            return memberExpression?.Member as PropertyInfo;
        }

        public abstract void Configure();

        internal void SetDefaultsForProperties()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            int index = 0;
            properties.ForEach(prop => _propertyConfigurations.TryAdd(prop.Name, new ResourcePropertyConfiguration(prop.Name, prop.PropertyType, index++)));
        }

        internal void SetConventionsForProperties()
        {
            // Set column called [resource_type_name]Id to primary identifier (i.e. Person -> PersonId)
            var idPropName = typeof(T).Name.ToLowerInvariant() + "id";
            var entityIds = _propertyConfigurations.Where(prop => prop.Key.ToLower() == idPropName);
            entityIds.FirstOrDefault().Value?.IsPrimaryIdentifier(true);

            // Set column called Id to primary identifier
            var ids = _propertyConfigurations.Where(prop => prop.Key.ToLower() == "id");
            ids.FirstOrDefault().Value?.IsPrimaryIdentifier(true);
        }

        internal void SetConventionsForResource()
        {
            // Set table name to the same as resource name
            _resourceConfiguration.HasTableName(typeof(T).Name);
        }

        public IRuntimeResourceConfiguration<T> BuildRuntimeConfiguration()
        {
            SetDefaultsForProperties();
            SetConventionsForProperties();
            SetConventionsForResource();

            Configure();

            return new ResourceConfiguration<T>(_resourceConfiguration.TableName, GetPropertyConfigurationsForRead());
        }

        private IEnumerable<PropertyConfig> GetPropertyConfigurationsForRead()
        {
            return _propertyConfigurations
                .Select(prop => new PropertyConfig(
                    prop.Value.Index,
                    prop.Value.ColumnName,
                    prop.Value.PropertyName,
                    prop.Value.PropertyType,
                    prop.Value.IsPrimaryResourceIdentifier,
                    prop.Value.IsReadOnly,
                    prop.Value.SqlDbType));
        }
    }

    public interface IResourceConfigurationBuilder<out T>
        where T : class, IRestResource
    {
       IRuntimeResourceConfiguration<T> BuildRuntimeConfiguration();
    }

    public class ResourceConfiguration<T> : IRuntimeResourceConfiguration<T>
        where T : class, IRestResource
    {
        private readonly IEnumerable<PropertyConfig> _propertyConfigurations;

        public ResourceConfiguration(
            string tableName,
            IEnumerable<PropertyConfig> propertyConfigurations)
        {
            TableName = tableName;
            _propertyConfigurations = propertyConfigurations;
        }

        public string TableName { get; }

        public IEnumerable<PropertyConfig> PropertyConfigurationsForRead => _propertyConfigurations.OrderBy(prop => prop.Index);

        public PropertyConfig PrimaryIdentifier => _propertyConfigurations.Single(prop => prop.IsPrimaryIdentifier);

        public IEnumerable<PropertyConfig> PropertyConfigurationsForWrite => _propertyConfigurations.OrderBy(prop => prop.Index).Where(prop => !prop.IsReadOnly);
    }

    public interface IRuntimeResourceConfiguration<out T>
        where T : class, IRestResource
    {

        string TableName { get; }

        IEnumerable<PropertyConfig> PropertyConfigurationsForRead { get; }

        PropertyConfig PrimaryIdentifier { get; }

        IEnumerable<PropertyConfig> PropertyConfigurationsForWrite { get; }
    }

    public class PropertyConfig
    {
        public PropertyConfig(int index, string columnName, string propertyName, Type propertyType, bool isPrimaryIdentifier, bool isReadOnly, SqlDbType sqlDbType)
        {
            Index = index;
            ColumnName = columnName;
            PropertyName = propertyName;
            PropertyType = propertyType;
            IsPrimaryIdentifier = isPrimaryIdentifier;
            IsReadOnly = isReadOnly;
            SqlDbType = sqlDbType;
        }

        public int Index { get; }
        public string ColumnName { get; }
        public string PropertyName { get; }
        public Type PropertyType { get; }
        public bool IsPrimaryIdentifier { get; }
        public bool IsReadOnly { get; }
        public SqlDbType SqlDbType { get; }
    }

    public class ResourcePropertyConfiguration
    {
        internal string PropertyName { get; }

        internal Type PropertyType { get; }

        internal int Index { get; }

        internal SqlDbType SqlDbType { get; private set; }

        internal string ColumnName { get; private set; }

        internal bool IsReadOnly { get; private set; } = false;

        internal bool IsPrimaryResourceIdentifier { get; private set; } = false;

        public ResourcePropertyConfiguration(string propertyName, Type propertyType, int index)
        {
            propertyType.TryUnwrapNullableType(out Type unwrappedType);

            PropertyName = propertyName;
            PropertyType = propertyType;
            ColumnName = propertyName;
            Index = index;
            SqlDbType = Maps.DefaultSqlDbTypeMap.GetValueOrDefault(unwrappedType)
                ?? throw new ArgumentException($"Property of type {propertyType.Name} cannot be used.", nameof(propertyType));
        }

        public ResourcePropertyConfiguration IsReadOnlyColumn(bool isReadOnly = true)
        {
            if (!isReadOnly && IsPrimaryResourceIdentifier)
            {
                throw new InvalidOperationException("Primary identifier column must always be read only.");
            }

            IsReadOnly = isReadOnly;
            return this;
        }

        public ResourcePropertyConfiguration HasSqlDbType(SqlDbType sqlDbType)
        {
            SqlDbType = sqlDbType;
            return this;
        }

        public ResourcePropertyConfiguration HasColumnName(string columnName)
        {
            ColumnName = columnName;
            return this;
        }
        
        public ResourcePropertyConfiguration IsPrimaryIdentifier(bool isPrimaryIdentifier = true)
        {
            IsPrimaryResourceIdentifier = isPrimaryIdentifier;
            IsReadOnlyColumn(true);
            return this;
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

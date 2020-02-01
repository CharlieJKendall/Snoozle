using Snoozle.Abstractions;
using Snoozle.SqlServer.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Snoozle.SqlServer.Configuration
{
    public abstract class SqlResourceConfigurationBuilder<TResource> : BaseResourceConfigurationBuilder<TResource, ISqlPropertyConfiguration, ISqlResourceConfiguration, ISqlModelConfiguration>
        where TResource : class, IRestResource
    {
        protected override IPropertyConfigurationBuilder<TProperty, ISqlPropertyConfiguration> CreatePropertyConfigurationBuilder<TProperty>(
            ISqlPropertyConfiguration propertyConfiguration)
        {
            return new SqlPropertyConfigurationBuilder<TResource, TProperty>(propertyConfiguration);
        }

        protected override ISqlResourceConfiguration CreateResourceConfiguration()
        {
            return new SqlResourceConfiguration<TResource>(ModelConfiguration, PropertyConfigurations);
        }

        protected override ISqlModelConfiguration CreateModelConfiguration()
        {
            return new SqlModelConfiguration<TResource>();
        }

        protected override ISqlPropertyConfiguration CreatePropertyConfiguration()
        {
            return new SqlPropertyConfiguration();
        }

        protected override IModelConfigurationBuilder<ISqlModelConfiguration> CreateModelConfigurationBuilder()
        {
            return new SqlModelConfigurationBuilder(ModelConfiguration);
        }

        protected override void SetPropertyConfigurationDefaults()
        {
            base.SetPropertyConfigurationDefaults();

            foreach (ISqlPropertyConfiguration propertyConfig in PropertyConfigurations)
            {
                propertyConfig.SqlDbType = GetDefaultSqlDbType(propertyConfig.PropertyType);
                propertyConfig.ColumnName = propertyConfig.PropertyName;
            }
        }

        private SqlDbType? GetDefaultSqlDbType(Type type)
        {
            type.TryUnwrapNullableType(out Type unwrappedType);
            return Maps.DefaultSqlDbTypeMap.GetValueOrDefault(unwrappedType);
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

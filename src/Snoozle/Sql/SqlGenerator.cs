using Snoozle.Core;
using Snoozle.Extensions;
using Snoozle.RestResourceConfiguration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Snoozle.Sql
{
    public class SqlGenerator : ISqlGenerator
    {
        private readonly ISqlParamaterProvider _sqlParamaterProvider;

        public SqlGenerator(ISqlParamaterProvider sqlParamaterProvider)
        {
            _sqlParamaterProvider = sqlParamaterProvider;
        }

        public string SelectAll(IRuntimeResourceConfiguration config)
        {
            return SelectAllBuilder(config).ToString();
        }

        public string DeleteById(IRuntimeResourceConfiguration config)
        {
            return new StringBuilder($"DELETE FROM [{config.TableName}]")
                .AppendWhereClause(config.PrimaryIdentifier.ColumnName, _sqlParamaterProvider.GetPrimaryKeyParameterName())
                .ToString();
        }

        private StringBuilder SelectAllBuilder(IRuntimeResourceConfiguration config)
        {
            StringBuilder stringBuilder = new StringBuilder("SELECT ");
            IResourcePropertyConfiguration[] properties = config.PropertyConfigurationsForRead.ToArray();

            for (int i = 0; i < properties.Length; i++)
            {
                stringBuilder.Append("[");
                stringBuilder.Append(properties[i].ColumnName);
                stringBuilder.Append("] AS [");
                stringBuilder.Append(properties[i].PropertyName);
                stringBuilder.Append("]");

                if (i != properties.Length - 1)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(" ");
            }

            stringBuilder.Append("FROM [");
            stringBuilder.Append(config.TableName);
            stringBuilder.Append("]");

            return stringBuilder;
        }

        public string SelectById(IRuntimeResourceConfiguration config)
        {
            return SelectByIdBuilder(config).ToString();
        }

        public StringBuilder SelectByIdBuilder(IRuntimeResourceConfiguration config)
        {
            return SelectAllBuilder(config)
                .AppendWhereClause(config.PrimaryIdentifier.ColumnName, _sqlParamaterProvider.GetPrimaryKeyParameterName());
        }

        public string Insert(IRuntimeResourceConfiguration config)
        {
            IResourcePropertyConfiguration[] properties = config.PropertyConfigurationsForWrite.ToArray();
            StringBuilder stringBuilder = new StringBuilder("INSERT INTO [");
            stringBuilder.Append(config.TableName);
            stringBuilder.Append("] (");

            for (int i = 0; i < properties.Length; i++)
            {
                stringBuilder.Append("[");
                stringBuilder.Append(properties[i].ColumnName);
                stringBuilder.Append("]");

                if (i != properties.Length - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append(") VALUES (");

            for (int i = 0; i < properties.Length; i++)
            {
                stringBuilder.Append(_sqlParamaterProvider.GenerateParameterName(properties[i].PropertyName));

                if (i != properties.Length - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append(") ");

            stringBuilder.Append(
                SelectAllBuilder(config)
                .AppendWhereClause(config.PrimaryIdentifier.ColumnName, "SCOPE_IDENTITY()"));

            return stringBuilder.ToString();
        }

        public string Update(IRuntimeResourceConfiguration config)
        {
            IResourcePropertyConfiguration[] properties = config.PropertyConfigurationsForWrite.ToArray();
            StringBuilder stringBuilder = new StringBuilder("UPDATE [");
            stringBuilder.Append(config.TableName);
            stringBuilder.Append("] SET ");

            for (int i = 0; i < properties.Length; i++)
            {
                stringBuilder.Append("[");
                stringBuilder.Append(properties[i].ColumnName);
                stringBuilder.Append("] = ");
                stringBuilder.Append(_sqlParamaterProvider.GenerateParameterName(properties[i].PropertyName));

                if (i != properties.Length - 1)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(" ");
            }

            return stringBuilder
                .AppendWhereClause(config.PrimaryIdentifier.ColumnName, _sqlParamaterProvider.GetPrimaryKeyParameterName())
                .Append(SelectByIdBuilder(config)).ToString();
        }
    }

    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendWhereClause(this StringBuilder @this, string columnName, string parameterName)
        {
            @this.Append(" WHERE [");
            @this.Append(columnName);
            @this.Append("] = ");
            @this.Append(parameterName);
            @this.Append(" ");

            return @this;
        }
    }

    public interface ISqlParamaterProvider
    {
        string GenerateParameterName(string propertyName);
        string GetPrimaryKeyParameterName();
    }

    public class SqlParameterProvider : ISqlParamaterProvider
    {
        private const string ID_PARAM_NAME = "PrimaryKeyIdParam";

        public string GenerateParameterName(string propertyName)
        {
            if (propertyName == ID_PARAM_NAME)
            {
                throw new ArgumentException($"Property cannot be called '{ID_PARAM_NAME}'; this is reserved for internal usage.", nameof(propertyName));
            }

            return $"@{propertyName}";
        }

        public string GetPrimaryKeyParameterName()
        {
            return $"@{ID_PARAM_NAME}";
        }
    }

    public interface ISqlGenerator
    {
        string SelectAll(IRuntimeResourceConfiguration config);

        string SelectById(IRuntimeResourceConfiguration config);

        string DeleteById(IRuntimeResourceConfiguration config);

        string Insert(IRuntimeResourceConfiguration config);

        string Update(IRuntimeResourceConfiguration config);
    }

    public class SqlExpressionBuilder : ISqlExpressionBuilder
    {
        private readonly ISqlParamaterProvider _sqlParamaterProvider;

        public SqlExpressionBuilder(ISqlParamaterProvider sqlParamaterProvider)
        {
            _sqlParamaterProvider = sqlParamaterProvider;
        }

        public Func<object, object> GetPrimaryKeyValue<T>(IRuntimeResourceConfiguration config)
            where T : class, IRestResource
        {
            var paramResource = Expression.Parameter(typeof(object), "resource");
            var property = Expression.Convert(
                Expression.Property(Expression.Convert(paramResource, typeof(T)), config.PrimaryIdentifier.PropertyName),
                typeof(object));

            var lambda = Expression.Lambda<Func<object, object>>(
                property,
                paramResource);
            
            return lambda.Compile();
        }

        public Func<object, SqlParameter> GetPrimaryKeySqlParameter(IResourcePropertyConfiguration primaryIdentifierConfig)
        {
            return GetSqlParameter(primaryIdentifierConfig, _sqlParamaterProvider.GetPrimaryKeyParameterName()).Compile();
        }

        public Func<object, List<SqlParameter>> GetNonPrimaryKeySqlParameters<T>(IRuntimeResourceConfiguration config)
            where T : class, IRestResource
        {
            var configs = config.PropertyConfigurationsForWrite.ToArray();

            var paramResource = Expression.Parameter(typeof(object), "resourceObject");
            var typedParam = Expression.Convert(paramResource, typeof(T));
            var result = Expression.Variable(typeof(List<SqlParameter>), "sqlParameters");
            var typedParamVar = Expression.Variable(typeof(T), "typedParam");
            var assignTypedParamVar = Expression.Assign(typedParamVar, typedParam);
            var instantiateResultObj = Expression.Assign(result, Expression.New(typeof(List<SqlParameter>)));

            List<Expression> expressions = new List<Expression>
            {
                assignTypedParamVar,
                instantiateResultObj
            };

            for (int i = 0; i < configs.Length; i++)
            {
                MemberExpression property = Expression.Property(typedParamVar, configs[i].PropertyName);
                Expression<Func<object, SqlParameter>> sqlParam = GetSqlParameter(configs[i], _sqlParamaterProvider.GenerateParameterName(configs[i].PropertyName));
                InvocationExpression getSqlParam = Expression.Invoke(sqlParam, Expression.Convert(property, typeof(object)));
                MethodCallExpression generateNameAndAddToList = Expression.Call(
                    result,
                    nameof(List<SqlParameter>.Add),
                    null,
                    getSqlParam);

                expressions.Add(generateNameAndAddToList);
            }

            // Return the result from the expression
            expressions.Add(result);

            var lambda = Expression.Lambda<Func<object, List<SqlParameter>>>(
                Expression.Block(
                    new[] { result, typedParamVar },
                    expressions),
                paramResource);
            
            return lambda.Compile();
        }

        private Expression<Func<object, SqlParameter>> GetSqlParameter(IResourcePropertyConfiguration config, string paramName)
        {
            return (value) =>
                new SqlParameter(paramName, value ?? DBNull.Value)
                {
                    SqlDbType = config.SqlDbType.Value,
                    IsNullable = true
                };
        }

        public Func<SqlDataReader, T> CreateObjectRelationalMap<T>(IRuntimeResourceConfiguration config)
            where T : class, IRestResource
        {
            var orderedConfigs = config.PropertyConfigurationsForRead.OrderBy(prop => prop.Index).ToArray();

            var paramDataReader = Expression.Parameter(typeof(SqlDataReader), "sqlDataReader");
            var result = Expression.Variable(typeof(T), "result");
            var instantiateResultObj = Expression.Assign(result, Expression.New(typeof(T)));

            List<Expression> expressions = new List<Expression>
            {
                instantiateResultObj
            };

            // Add assignment expression for each property that uses correct read method on SqlDataReader for the property type
            for (int i = 0; i < orderedConfigs.Length; i++)
            {
                var property = Expression.Property(result, orderedConfigs[i].PropertyName);
                var assignProperty = Expression.IfThenElse(
                    Expression.Call(paramDataReader, nameof(SqlDataReader.IsDBNull), null, Expression.Constant(i)),
                    Expression.Assign(
                        Expression.MakeMemberAccess(result, property.Member),
                        Expression.Default(property.Type)),
                    Expression.Assign(
                        Expression.MakeMemberAccess(result, property.Member),
                        GetMethodCallForType(paramDataReader, Expression.Constant(i), property)));

                expressions.Add(assignProperty);
            }

            // Return the result from the expression
            expressions.Add(result);

            var lambda = Expression.Lambda<Func<SqlDataReader, T>>(
                Expression.Block(
                    new[] { result },
                    expressions),
                paramDataReader);
            
            return lambda.Compile();
        }

        private Expression GetMethodCallForType(Expression dataReaderInstance, Expression dataIndex, MemberExpression property)
        {
            bool wasUnwrapped = property.Type.TryUnwrapNullableType(out Type unwrappedTypeOrOriginal);

            switch (unwrappedTypeOrOriginal)
            {
                case Type _ when unwrappedTypeOrOriginal == typeof(string):
                    return GetMethodCallWithCast(wasUnwrapped, dataReaderInstance, nameof(SqlDataReader.GetString), dataIndex, property);
                case Type _ when unwrappedTypeOrOriginal == typeof(int):
                    return GetMethodCallWithCast(wasUnwrapped, dataReaderInstance, nameof(SqlDataReader.GetInt32), dataIndex, property);
                case Type _ when unwrappedTypeOrOriginal == typeof(DateTime):
                    return GetMethodCallWithCast(wasUnwrapped, dataReaderInstance, nameof(SqlDataReader.GetDateTime), dataIndex, property);
                case Type _ when unwrappedTypeOrOriginal == typeof(double):
                    return GetMethodCallWithCast(wasUnwrapped, dataReaderInstance, nameof(SqlDataReader.GetDouble), dataIndex, property);
                case Type _ when unwrappedTypeOrOriginal == typeof(float):
                    return GetMethodCallWithCast(wasUnwrapped, dataReaderInstance, nameof(SqlDataReader.GetFloat), dataIndex, property);
                case Type _ when unwrappedTypeOrOriginal == typeof(long):
                    return GetMethodCallWithCast(wasUnwrapped, dataReaderInstance, nameof(SqlDataReader.GetInt64), dataIndex, property);
                default:
                    throw new NotSupportedException($"Type {unwrappedTypeOrOriginal.Name} is not supported.");
            }
        }

        private Expression GetMethodCallWithCast(bool isNullableValueType, Expression dataReaderInstance, string dataReaderMethodName, Expression dataIndex, MemberExpression property)
        {
            if (isNullableValueType)
            {
                return Expression.Convert(Expression.Call(dataReaderInstance, dataReaderMethodName, null, dataIndex), property.Type);
            }
            else
            { 
                return Expression.Call(dataReaderInstance, dataReaderMethodName, null, dataIndex);
            }
        }
    }

    public interface ISqlExpressionBuilder
    {
        Func<object, SqlParameter> GetPrimaryKeySqlParameter(IResourcePropertyConfiguration primaryIdentifierConfig);

        Func<SqlDataReader, T> CreateObjectRelationalMap<T>(IRuntimeResourceConfiguration config)
            where T : class, IRestResource;

        Func<object, List<SqlParameter>> GetNonPrimaryKeySqlParameters<T>(IRuntimeResourceConfiguration config)
            where T : class, IRestResource;

        Func<object, object> GetPrimaryKeyValue<T>(IRuntimeResourceConfiguration config)
            where T : class, IRestResource;
    }
}

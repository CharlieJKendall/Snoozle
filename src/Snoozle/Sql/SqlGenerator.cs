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
        public string SelectAll<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource
        {
            return SelectAllBuilder(config).ToString();
        }

        public string DeleteById<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource
        {
            return $"DELETE FROM [{config.TableName}] WHERE [{config.PrimaryIdentifier.ColumnName}] = {SqlConstants.ID_PARAM_NAME}";
        }

        private StringBuilder SelectAllBuilder<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource
        {
            StringBuilder stringBuilder = new StringBuilder("SELECT ");
            PropertyConfig[] map = config.PropertyConfigurationsForRead.ToArray();

            for (int i = 0; i < map.Length; i++)
            {
                stringBuilder.Append("[");
                stringBuilder.Append(map[i].ColumnName);
                stringBuilder.Append("] AS [");
                stringBuilder.Append(map[i].PropertyName);
                stringBuilder.Append("]");

                if (i != map.Length - 1)
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

        public string SelectById<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource
        {
            return SelectByIdBuilder(config).ToString();
        }

        private StringBuilder SelectByIdBuilder<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource
        {
            StringBuilder stringBuilder = SelectAllBuilder(config);
            stringBuilder.Append(" WHERE [");
            stringBuilder.Append(config.PrimaryIdentifier.ColumnName);
            stringBuilder.Append("] = ");
            stringBuilder.Append(SqlConstants.ID_PARAM_NAME);

            return stringBuilder;
        }
    }

    public interface ISqlGenerator
    {
        string SelectAll<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource;
        string SelectById<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource;
        string DeleteById<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource;
    }

    public class SqlExpressionBuilder : ISqlExpressionBuilder
    {
        public Expression<Func<object, SqlParameter>> GetPrimaryKeySqlParameter(PropertyConfig primaryIdentifierConfig)
        {
            return (primaryKey) =>
                new SqlParameter(SqlConstants.ID_PARAM_NAME, primaryKey)
                {
                    SqlDbType = primaryIdentifierConfig.SqlDbType
                };            
        }

        public Func<SqlDataReader, T> CreateObjectRelationalMapFunc<T>(IRuntimeResourceConfiguration<T> config)
            where T : class, IRestResource
        {
            return CreateObjectRelationalMap(config).Compile();
        }

        public Expression<Func<SqlDataReader, T>> CreateObjectRelationalMap<T>(IRuntimeResourceConfiguration<T> config)
            where T : class, IRestResource
        {
            var orderedConfigs = config.PropertyConfigurationsForRead.OrderBy(prop => prop.Index).ToArray();

            var paramDataReader = Expression.Parameter(typeof(SqlDataReader), "sqlDataReader");
            var result = Expression.Variable(typeof(T), "result");
            var instantiateResultObj = Expression.Assign(result, Expression.New(typeof(T)));

            List<Expression> expressions = new List<Expression>
            {
                paramDataReader,
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

            return Expression.Lambda<Func<SqlDataReader, T>>(
                Expression.Block(
                    new[] { result },
                    expressions),
                paramDataReader);
        }

        public Expression GetMethodCallForType(Expression dataReaderInstance, Expression dataIndex, MemberExpression property)
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
        Expression<Func<object, SqlParameter>> GetPrimaryKeySqlParameter(PropertyConfig primaryIdentifierConfig);
        Expression<Func<SqlDataReader, T>> CreateObjectRelationalMap<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource;
        Func<SqlDataReader, T> CreateObjectRelationalMapFunc<T>(IRuntimeResourceConfiguration<T> config) where T : class, IRestResource;
    }
}

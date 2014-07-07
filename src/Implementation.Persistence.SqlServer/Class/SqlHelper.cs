namespace Implementation.Persistence.SqlServer.Class
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq.Expressions;

    using global::Implementation.Persistence.SqlServer.Extension;

    using Model.Persistence.Class;
    using Model.Persistence.Interface;

    using Shared.Extension;

    public class SqlHelper
    {
        #region Fields

        private readonly string connectionString;

        #endregion

        #region Constructors and Destructors

        public SqlHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion

        #region Public Methods and Operators

        public static SqlParameter Parameter(
            string name,
            object value,
            Type type,
            ParameterDirection direction = ParameterDirection.Input)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            if (direction == ParameterDirection.Output)
            {
                if (ReferenceEquals(type, typeof(string)))
                {
                    return new SqlParameter(name, SqlDbType.VarChar, -1) { Direction = direction };
                }

                if (ReferenceEquals(type, typeof(byte[])))
                {
                    return new SqlParameter(name, SqlDbType.VarBinary, -1) { Direction = direction };
                }
            }

            if (value != default(object))
            {
                if (!ReferenceEquals(value.GetType(), type))
                {
                    throw new ArgumentException("Resource type does not match type.").AddDumpObject(() => name)
                        .AddDumpObject(() => value)
                        .AddDumpObject(() => type);
                }

                if (ReferenceEquals(type, typeof(Guid)))
                {
                    value = ((Guid)value).ToByteArray();
                }
                else if (ReferenceEquals(type, typeof(DateTime)))
                {
                    var dateTime = value as DateTime?;
                    if (dateTime.HasValue)
                    {
                        if (dateTime.Value < SqlParameterExtension.MinSqlDateTimeValue)
                        {
                            value = SqlParameterExtension.MinSqlDateTimeValue;
                        }
                        else if (dateTime.Value > SqlParameterExtension.MaxSqlDateTimeValue)
                        {
                            value = SqlParameterExtension.MaxSqlDateTimeValue;
                        }
                        else
                        {
                            value = dateTime.Value.ToUniversalTime();
                        }
                    }
                    else
                    {
                        value = SqlParameterExtension.MinSqlDateTimeValue;
                    }
                }
                else if (ReferenceEquals(type, typeof(DateTimeOffset)))
                {
                    var dateTimeOffset = value as DateTimeOffset?;
                    if (dateTimeOffset.HasValue)
                    {
                        if (dateTimeOffset.Value < SqlParameterExtension.MinSqlDateTimeValue)
                        {
                            value = SqlParameterExtension.MinSqlDateTimeValue;
                        }
                        else if (dateTimeOffset.Value > SqlParameterExtension.MaxSqlDateTimeValue)
                        {
                            value = SqlParameterExtension.MaxSqlDateTimeValue;
                        }
                        else
                        {
                            value = dateTimeOffset.Value.UtcDateTime;
                        }
                    }
                    else
                    {
                        value = SqlParameterExtension.MinSqlDateTimeValue;
                    }
                }

                return new SqlParameter(name, value) { Direction = direction };
            }

            // Null values.
            if (ReferenceEquals(type, typeof(Guid)))
            {
                return new SqlParameter(name, SqlDbType.Binary)
                {
                    Direction = direction,
                    Size = 16,
                    Value = DBNull.Value
                };
            }

            SqlDbType dbType;
            if (ReferenceEquals(type, typeof(bool)))
            {
                dbType = SqlDbType.Bit;
            }
            else if (ReferenceEquals(type, typeof(byte[])))
            {
                dbType = SqlDbType.VarBinary;
            }
            else if (ReferenceEquals(type, typeof(DateTime)))
            {
                dbType = SqlDbType.DateTime;
            }
            else if (ReferenceEquals(type, typeof(DateTimeOffset)))
            {
                dbType = SqlDbType.DateTimeOffset;
            }
            else if (ReferenceEquals(type, typeof(short)))
            {
                dbType = SqlDbType.SmallInt;
            }
            else if (ReferenceEquals(type, typeof(int)))
            {
                dbType = SqlDbType.Int;
            }
            else if (ReferenceEquals(type, typeof(long)))
            {
                dbType = SqlDbType.BigInt;
            }
            else if (ReferenceEquals(type, typeof(string)))
            {
                dbType = SqlDbType.VarChar;
            }
            else
            {
                throw new NotImplementedException(
                    "Don't know how to convert type to SqlDbType. {0}".FormatWith(type.Name));
            }

            return new SqlParameter(name, dbType) { Direction = direction, Value = DBNull.Value };
        }

        public static SqlParameter Parameter<T>(string name, ParameterDirection direction = ParameterDirection.Output)
        {
            return Parameter(name, default(object), typeof(T), direction);
        }

        public static SqlParameter Parameter<T>(
            Expression<Func<T>> expression,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var body = (MemberExpression)expression.Body;
            T value = expression.Compile().Invoke();
            return Parameter('@' + body.Member.Name.ToFirstCharacterLower(), value, typeof(T), direction);
        }

        public int ExecuteStoredProcedure(string storedProcedureName, params SqlParameter[] parameters)
        {
            using (var sqlConnection = new SqlConnection(this.connectionString))
            {
                using (var sqlCommand = new SqlCommand(storedProcedureName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    var returnValueParameter = new SqlParameter("@returnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    sqlCommand.Parameters.Add(returnValueParameter);
                    sqlCommand.Parameters.AddRange(parameters);

                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();

                    return returnValueParameter.CastTo<int>();
                }
            }
        }

        public int ExecuteStoredProcedureChannels(
            string storedProcedureName,
            out IEnumerable<IChannel> channels,
            params SqlParameter[] parameters)
        {
            var channelList = new List<IChannel>();
            channels = channelList;
            using (var sqlConnection = new SqlConnection(this.connectionString))
            {
                using (var sqlCommand = new SqlCommand(storedProcedureName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    var returnValueParameter = new SqlParameter("@returnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    sqlCommand.Parameters.Add(returnValueParameter);
                    sqlCommand.Parameters.AddRange(parameters);

                    sqlConnection.Open();
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var link = new Uri(sqlDataReader.GetString(1));
                            var rss = new Uri(sqlDataReader.GetString(2));
                            var channel = new Channel(sqlDataReader.GetGuid(0), link, rss, sqlDataReader.GetString(3));
                            channelList.Add(channel);
                        }
                    }

                    return returnValueParameter.CastTo<int>();
                }
            }
        }

        public int ExecuteStoredProcedureItems(
            string storedProcedureName,
            out IEnumerable<IItem> items,
            params SqlParameter[] parameters)
        {
            var itemList = new List<IItem>();
            items = itemList;
            using (var sqlConnection = new SqlConnection(this.connectionString))
            {
                using (var sqlCommand = new SqlCommand(storedProcedureName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    var returnValueParameter = new SqlParameter("@returnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    sqlCommand.Parameters.Add(returnValueParameter);
                    sqlCommand.Parameters.AddRange(parameters);

                    sqlConnection.Open();
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var link = new Uri(sqlDataReader.GetString(2));
                            var item = new Item(
                                sqlDataReader.GetGuid(0),
                                sqlDataReader.GetString(1),
                                link,
                                sqlDataReader.GetInt32(3),
                                sqlDataReader.GetString(4));
                            itemList.Add(item);
                        }
                    }

                    return returnValueParameter.CastTo<int>();
                }
            }
        }

        #endregion
    }
}
namespace Implementation.Persistence.SqlServer.Extension
{
    using System;
    using System.Data.SqlClient;

    public static class SqlParameterExtension
    {
        #region Static Fields

        public static readonly DateTime MaxSqlDateTimeValue = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        public static readonly DateTime MinSqlDateTimeValue = new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods and Operators

        public static T CastTo<T>(this SqlParameter @this)
        {
            if (Equals(@this.Value, DBNull.Value))
            {
                return default(T);
            }

            if (ReferenceEquals(typeof(T), typeof(Guid)))
            {
                return (T)(object)new Guid((byte[])@this.Value);
            }

            if (ReferenceEquals(typeof(T), typeof(DateTime)))
            {
                DateTime dateTime = DateTime.SpecifyKind((DateTime)@this.Value, DateTimeKind.Utc);
                if (dateTime == MaxSqlDateTimeValue)
                {
                    dateTime = DateTime.MaxValue;
                }
                else if (dateTime == MinSqlDateTimeValue)
                {
                    dateTime = DateTime.MinValue;
                }

                return (T)(object)dateTime;
            }

            if (!ReferenceEquals(typeof(T), typeof(DateTimeOffset)))
            {
                return (T)Convert.ChangeType(@this.Value, typeof(T));
            }

            var dateTimeOffset = (DateTimeOffset)Convert.ChangeType(@this.Value, typeof(DateTimeOffset));
            if (dateTimeOffset == MaxSqlDateTimeValue)
            {
                dateTimeOffset = DateTimeOffset.MaxValue;
            }
            else if (dateTimeOffset == MinSqlDateTimeValue)
            {
                dateTimeOffset = DateTimeOffset.MinValue;
            }

            return (T)(object)dateTimeOffset;
        }

        #endregion
    }
}
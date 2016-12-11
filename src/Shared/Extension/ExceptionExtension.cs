namespace Shared.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Net;

    using Shared.Exceptions;

    public static class ExceptionExtension
    {
        #region Constants

        public const string ExceptionDataDumpKey = "DumpKey";

        #endregion

        #region Static Fields

        private static readonly Dictionary<Type, HttpStatusCode> ExceptionTypeToHttpStatusCode =
            new Dictionary<Type, HttpStatusCode>
            {
                { typeof(BadRequestException), HttpStatusCode.BadRequest },
                { typeof(ConflictException), HttpStatusCode.Conflict },
                { typeof(ForbiddenException), HttpStatusCode.Forbidden },
                { typeof(InternalServerErrorException), HttpStatusCode.InternalServerError },
                { typeof(NotFoundException), HttpStatusCode.NotFound },
                { typeof(UnauthorizedException), HttpStatusCode.Unauthorized },
            };

        #endregion

        #region Public Methods and Operators

        public static Exception AddDumpObject(this Exception @this, string name, object value)
        {
            IDictionary<string, object> dumpObject = @this.GetDumpObjects();
            if (dumpObject == default(Dictionary<string, object>))
            {
                dumpObject = new Dictionary<string, object>();
                @this.Data[ExceptionDataDumpKey] = dumpObject;
            }

            dumpObject[name] = value;

            return @this;
        }

        public static Exception AddDumpObject<T>(this Exception @this, Expression<Func<T>> expression)
        {
            var body = expression.Body as MemberExpression;
            string name = body != default(MemberExpression) ? body.Member.Name : expression.Body.Type.Name;
            object value = expression.Compile().Invoke();
            var type = value as Type;
            if (type != default(Type))
            {
                value = type.FullName;
            }

            @this.AddDumpObject(name, value);
            return @this;
        }

        public static IDictionary<string, object> GetDumpObjects(this Exception @this)
        {
            return @this.Data.Contains(ExceptionDataDumpKey)
                ? @this.Data[ExceptionDataDumpKey] as IDictionary<string, object>
                : default(IDictionary<string, object>);
        }

        public static HttpStatusCode ToStatusCode(this Exception @this)
        {
            HttpStatusCode httpStatusCode;
            if (!ExceptionTypeToHttpStatusCode.TryGetValue(@this.GetType(), out httpStatusCode))
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            return httpStatusCode;
        }

        #endregion
    }
}
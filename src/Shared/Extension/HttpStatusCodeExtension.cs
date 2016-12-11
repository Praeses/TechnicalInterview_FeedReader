namespace Shared.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Shared.Exceptions;

    public static class HttpStatusCodeExtension
    {
        #region Static Fields

        private static readonly Dictionary<HttpStatusCode, Type> HttpStatusCodeToException =
            new Dictionary<HttpStatusCode, Type>
            {
                { HttpStatusCode.BadRequest, typeof(BadRequestException) },
                { HttpStatusCode.Conflict, typeof(ConflictException) },
                { HttpStatusCode.Forbidden, typeof(ForbiddenException) },
                { HttpStatusCode.InternalServerError, typeof(InternalServerErrorException) },
                { HttpStatusCode.NotFound, typeof(NotFoundException) },
                { HttpStatusCode.Unauthorized, typeof(UnauthorizedException) },
            };

        #endregion

        #region Public Methods and Operators

        public static Type ToExceptionType(this HttpStatusCode @this)
        {
            Type exceptionType;
            if (!HttpStatusCodeToException.TryGetValue(@this, out exceptionType))
            {
                exceptionType = typeof(InternalServerErrorException);
            }

            return exceptionType;
        }

        #endregion
    }
}
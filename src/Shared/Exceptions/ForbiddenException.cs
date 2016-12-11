namespace Shared.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ForbiddenException : Exception
    {
        #region Constructors and Destructors

        public ForbiddenException()
        {
        }

        public ForbiddenException(string message)
            : base(message)
        {
        }

        public ForbiddenException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
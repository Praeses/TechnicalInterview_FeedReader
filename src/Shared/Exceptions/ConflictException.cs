namespace Shared.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ConflictException : Exception
    {
        #region Constructors and Destructors

        public ConflictException()
        {
        }

        public ConflictException(string message)
            : base(message)
        {
        }

        public ConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
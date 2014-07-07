namespace Shared.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NotFoundException : Exception
    {
        #region Constructors and Destructors

        public NotFoundException()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
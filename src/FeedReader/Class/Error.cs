namespace FeedReader.Class
{
    using System;
    using System.Collections.Generic;

    using Shared.Extension;

    public class Error
    {
        #region Constructors and Destructors

        public Error(Exception exception)
        {
            this.DumpObjects = exception.GetDumpObjects();
            this.ExceptionType = exception.GetType().FullName;
            this.Message = exception.Message;

            if (exception.InnerException != default(Exception))
            {
                this.InnerException = new Error(exception.InnerException);
            }
        }

        #endregion

        #region Public Properties

        public IDictionary<string, object> DumpObjects { get; private set; }

        public string ExceptionType { get; private set; }

        public Error InnerException { get; private set; }

        public string Message { get; private set; }

        #endregion
    }
}
namespace FeedReader.Class
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;

    using Shared.Extension;

    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region Public Methods and Operators

        public override void OnException(HttpActionExecutedContext context)
        {
            HttpStatusCode httpStatusCode = context.Exception.ToStatusCode();
            context.Response = context.Request.CreateResponse(httpStatusCode, new Error(context.Exception));
        }

        #endregion
    }
}
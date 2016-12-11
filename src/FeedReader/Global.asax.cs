namespace FeedReader
{
    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FeedReader.Class;

    using Model.Api.Interface;
    using Model.Persistence.Interface;

    using SimpleInjector;

    public class MvcApplication : HttpApplication
    {
        #region Static Fields

        private static IAuthenticationService authenticationService;

        #endregion

        #region Methods

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            string xTokenHeader = this.Context.Request.Headers["X-Token"];
            if (string.IsNullOrEmpty(xTokenHeader))
            {
                return;
            }

            IToken token;
            Guid tokenGuid = Guid.Parse(xTokenHeader);
            IUser user = authenticationService.AuthenticateRequest(tokenGuid, out token);
            if (user == default(IUser))
            {
                return;
            }

            var identity = new Identity(token, user);
            this.Context.User = new GenericPrincipal(identity, new string[] { });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Container container = SimpleInjectorConfig.RegisterComponents();
            authenticationService = container.GetInstance<IAuthenticationService>();
        }

        #endregion
    }
}
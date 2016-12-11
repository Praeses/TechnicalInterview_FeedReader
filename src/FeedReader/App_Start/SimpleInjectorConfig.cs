namespace FeedReader
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web.Http;

    using Implementation.Api.Implementation;
    using Implementation.Persistence.SqlServer.Class;
    using Implementation.Persistence.SqlServer.Implementation;

    using Model.Api.Interface;
    using Model.Persistence.Interface;

    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;

    public static class SimpleInjectorConfig
    {
        #region Public Methods and Operators

        public static Container RegisterComponents()
        {
            // Set up the localdb data directory.
            var dataDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            dataDirectory = Path.Combine(dataDirectory, "..\\..\\Database");
            dataDirectory = Path.GetFullPath(dataDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            // Create the container and register all the api controllers.
            var container = new Container();
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            // Model.Api Registrations
            container.Register<IAuthenticationService, AuthenticationService>();
            container.Register<IChannelService, ChannelService>();
            container.Register<IUserItemService, UserItemService>();
            container.Register<IRegistrationService, RegistrationService>();

            // Model.Persistence Registations
            container.Register(
                () => new SqlHelper(ConfigurationManager.AppSettings["persistence"]),
                Lifestyle.Singleton);
            container.Register<IRssService, RssService>();
            container.Register<IUserService, UserService>();

            // Verify that everything is wired up properly.
            container.Verify();

            // Register the container as the default dependency resolver.
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            return container;
        }

        #endregion
    }
}
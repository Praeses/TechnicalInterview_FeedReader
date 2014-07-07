namespace Test.Implementation.Persistence.SqlServer
{
    using System;
    using System.IO;

    using global::Implementation.Persistence.SqlServer.Class;
    using global::Implementation.Persistence.SqlServer.Implementation;

    using global::Model.Persistence.Interface;

    using SimpleInjector;

    public static class SimpleInjectorConfig
    {
        #region Public Methods and Operators

        public static Container RegisterComponents()
        {
            string dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dataDirectory = Path.Combine(dataDirectory, "..\\..\\..\\..\\Database");
            dataDirectory = Path.GetFullPath(dataDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            var container = new Container();

            container.Register(
                () =>
                    new SqlHelper(
                        "Data Source=(LocalDb)\\v11.0;AttachDbFilename=|DataDirectory|\\FeedReader.mdf;Initial Catalog=FeedReader;Integrated Security=True"),
                Lifestyle.Singleton);
            container.Register<IRssService, RssService>();
            container.Register<IUserService, UserService>();

            container.Verify();

            return container;
        }

        #endregion
    }
}
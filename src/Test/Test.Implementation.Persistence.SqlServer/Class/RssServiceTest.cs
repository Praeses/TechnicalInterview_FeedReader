namespace Test.Implementation.Persistence.SqlServer.Class
{
    using global::Model.Persistence.Interface;

    using NUnit.Framework;

    using SimpleInjector;

    [TestFixture]
    public class RssServiceTest : Model.Persisence.Interface.RssServiceTest
    {
        #region Static Fields

        private static readonly Container Container = SimpleInjectorConfig.RegisterComponents();

        #endregion

        #region Constructors and Destructors

        public RssServiceTest()
            : base(Container.GetInstance<IRssService>())
        {
        }

        #endregion
    }
}
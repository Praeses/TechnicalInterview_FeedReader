namespace Test.Implementation.Persistence.SqlServer.Class
{
    using global::Model.Persistence.Interface;

    using NUnit.Framework;

    using SimpleInjector;

    [TestFixture]
    public class UserServiceTest : Model.Persisence.Interface.UserServiceTest
    {
        #region Static Fields

        private static readonly Container Container = SimpleInjectorConfig.RegisterComponents();

        #endregion

        #region Constructors and Destructors

        public UserServiceTest()
            : base(Container.GetInstance<IRssService>(), Container.GetInstance<IUserService>())
        {
        }

        #endregion
    }
}
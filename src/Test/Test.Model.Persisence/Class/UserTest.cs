namespace Test.Model.Persisence.Class
{
    using global::Model.Persistence.Class;
    using global::Model.Persistence.Interface;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class UserTest
    {
        #region Constants

        private const string TestPassword = "testPassword";

        private const string TestPasswordChanged = "testPasswordChanged";

        private const string TestUserName = "testUserName";

        #endregion

        #region Public Methods and Operators

        [Test]
        public void IsPasswordValid_Invalid_Successful()
        {
            IUser user = CreateTestUser();
            Assert.IsFalse(user.IsPasswordValid("bad"));
        }

        [Test]
        public void IsPasswordValid_Valid_Successful()
        {
            IUser user = CreateTestUser();
            Assert.IsTrue(user.IsPasswordValid(TestPassword));
        }

        [Test]
        public void SetPassword_Valid_Successful()
        {
            IUser user = CreateTestUser();
            user.SetPassword(TestPasswordChanged);
            Assert.IsTrue(user.IsPasswordValid(TestPasswordChanged));
        }

        #endregion

        #region Methods

        private static IUser CreateTestUser()
        {
            return new User(TestUserName, string.Empty).SetPassword(TestPassword);
        }

        #endregion
    }
}
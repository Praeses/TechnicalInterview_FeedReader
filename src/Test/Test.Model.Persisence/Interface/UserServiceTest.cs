namespace Test.Model.Persisence.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Model.Persistence.Class;
    using global::Model.Persistence.Enum;
    using global::Model.Persistence.Extension;
    using global::Model.Persistence.Interface;

    using NUnit.Framework;

    using Shared.Exceptions;

    // ReSharper disable InconsistentNaming
    public abstract class UserServiceTest
    {
        #region Constants

        private const string TestChannelTitle = "testChannelTitle";

        private const string TestChannelTitle2 = "testChannelTitle2";

        private const string TestPassword = "testPassword";

        private const string TestTokenName = "testTokenName";

        private const string TestUserName = "testUserName";

        #endregion

        #region Static Fields

        private static readonly Guid TestChannelGuid = Guid.Parse("72B49E77-6782-49c7-A3CE-6D5EEDB9FA67");

        private static readonly Guid TestChannelGuid2 = Guid.Parse("E601F14D-885A-4631-9E34-3190B0543C32");

        private static readonly Uri TestChannelLink = new Uri("http://test.com//channel.html");

        private static readonly Uri TestChannelLink2 = new Uri("http://test.com//channel2.html");

        private static readonly Uri TestChannelRss = new Uri("http://test.com//channel.rss");

        private static readonly Uri TestChannelRss2 = new Uri("http://test.com//channel2.rss");

        private static readonly Guid TestUserGuid = Guid.Parse("59FC101D-8E4E-4521-A797-DBF2B085367B");

        #endregion

        #region Fields

        private readonly IRssService rssService;

        private readonly IUserService userService;

        #endregion

        #region Constructors and Destructors

        protected UserServiceTest(IRssService rssService, IUserService userService)
        {
            this.rssService = rssService;
            this.userService = userService;
        }

        #endregion

        #region Public Methods and Operators

        [Test]
        public void AddChannel_Created_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            IChannel channel = CreateTestChannel();
            this.userService.AddChannel(TestUserGuid, channel);
            List<IChannel> channels = this.userService.EnumerateChannels(TestUserGuid).ToList();
            Assert.AreEqual(channels.Count(), 1);
            AssertAreEqual(channels[0], channel);
        }

        [Test]
        public void AddChannel_Updated_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            IChannel channel = CreateTestChannel();
            this.userService.AddChannel(TestUserGuid, channel);
            this.userService.AddChannel(TestUserGuid, channel);
            List<IChannel> channels = this.userService.EnumerateChannels(TestUserGuid).ToList();
            Assert.AreEqual(channels.Count(), 1);
            AssertAreEqual(channels[0], channel);
        }

        [Test]
        [ExpectedException(typeof(NotFoundException))]
        public void AddChannel_UserDoesNotExist_NotFoundException()
        {
            IChannel channel = CreateTestChannel();
            this.userService.AddChannel(TestUserGuid, channel);
        }

        [Test]
        public void AddToken_Created_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            var token = new Token(TestTokenName, TokenType.Authentication);
            this.userService.AddToken(TestUserGuid, token);
            IUser getUser = this.userService.GetUser(token.GetTokenGuid());
            AssertAreEqual(getUser, user);
        }

        [Test]
        public void AddToken_Updated_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            var token = new Token(TestTokenName, TokenType.Authentication);
            this.userService.AddToken(TestUserGuid, token);
            this.userService.AddToken(TestUserGuid, token);
            IUser getUser = this.userService.GetUser(token.GetTokenGuid());
            AssertAreEqual(getUser, user);
        }

        [Test]
        [ExpectedException(typeof(NotFoundException))]
        public void AddToken_UserDoesNotExist_NotFoundException()
        {
            var token = new Token(TestTokenName, TokenType.Authentication);
            this.userService.AddToken(TestUserGuid, token);
        }

        [Test]
        public void DeleteUser_Exists_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            this.userService.DeleteUser(TestUserGuid);
            IUser getUser = this.userService.GetUser(TestUserGuid);
            Assert.Null(getUser);
        }

        [Test]
        public void DeleteUser_NotFound_Successful()
        {
            this.userService.DeleteUser(TestUserGuid);
        }

        [Test]
        public void EnumerateChannels_Multiple_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            IChannel channel = CreateTestChannel();
            this.userService.AddChannel(TestUserGuid, channel);
            IChannel channel2 = CreateTestChannel2();
            this.userService.AddChannel(TestUserGuid, channel2);
            List<IChannel> channels = this.userService.EnumerateChannels(TestUserGuid).ToList();
            Assert.AreEqual(channels.Count(), 2);
            AssertContains(channels, channel);
            AssertContains(channels, channel2);
        }

        [Test]
        [ExpectedException(typeof(NotFoundException))]
        public void EnumerateChannels_UserDoesNotExist_NotFoundException()
        {
            this.userService.EnumerateChannels(TestUserGuid);
        }

        [Test]
        public void GetUser_Exists_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            IUser getUser = this.userService.GetUser(TestUserName);
            AssertAreEqual(user, getUser);
        }

        [Test]
        public void GetUser_NotFound_Successful()
        {
            IUser getUser = this.userService.GetUser(TestUserName);
            Assert.IsNull(getUser);
        }

        [Test]
        public void PutUser_Created_Successful()
        {
            bool existed;
            IUser user = CreateTestUser();
            this.userService.PutUser(user, out existed);
            Assert.IsFalse(existed);
        }

        [Test]
        public void PutUser_Updated_Successful()
        {
            bool existed;
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            this.userService.PutUser(user, out existed);
            Assert.IsTrue(existed);
        }

        [Test]
        public void RemoveChannel_Exists_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            IChannel channel = CreateTestChannel();
            this.userService.AddChannel(TestUserGuid, channel);
            this.userService.RemoveChannel(TestUserGuid, TestChannelGuid);
            List<IChannel> channels = this.userService.EnumerateChannels(TestUserGuid).ToList();
            Assert.IsEmpty(channels);
        }

        [Test]
        public void RemoveChannel_NotFound_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            this.userService.RemoveChannel(TestUserGuid, TestChannelGuid);
            List<IChannel> channels = this.userService.EnumerateChannels(TestUserGuid).ToList();
            Assert.IsEmpty(channels);
        }

        [Test]
        [ExpectedException(typeof(NotFoundException))]
        public void RemoveChannel_UserDoesNotExist_NotFoundException()
        {
            this.userService.RemoveChannel(TestUserGuid, TestChannelGuid);
        }

        [Test]
        public void RemoveToken_Exists_Successful()
        {
            IUser user = CreateTestUser();
            this.userService.PutUser(user);
            var token = new Token(TestTokenName, TokenType.Authentication);
            this.userService.AddToken(TestUserGuid, token);
            this.userService.RemoveToken(token.GetTokenGuid());
            IUser getUser = this.userService.GetUser(token.GetTokenGuid());
            Assert.IsNull(getUser);
        }

        [Test]
        public void RemoveToken_NotFound_Successful()
        {
            this.userService.RemoveToken(Guid.NewGuid());
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            this.rssService.DeleteChannel(TestChannelGuid);
            this.rssService.DeleteChannel(TestChannelGuid2);
            this.userService.DeleteUser(TestUserGuid);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            this.TearDown();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        #endregion

        #region Methods

        private static void AssertAreEqual(IChannel channelA, IChannel channelB)
        {
            Assert.IsNotNull(channelA);
            Assert.IsNotNull(channelB);
            Assert.AreEqual(channelA.GetChannelGuid(), channelB.GetChannelGuid());
            Assert.AreEqual(channelA.Link.AbsoluteUri, channelB.Link.AbsoluteUri);
            Assert.AreEqual(channelA.Rss.AbsoluteUri, channelB.Rss.AbsoluteUri);
            Assert.AreEqual(channelA.Title, channelB.Title);
        }

        private static void AssertAreEqual(IUser userA, IUser userB)
        {
            Assert.IsNotNull(userA);
            Assert.IsNotNull(userB);
            Assert.AreEqual(userA.HashedPassword, userB.HashedPassword);
            Assert.AreEqual(userA.GetUserGuid(), userB.GetUserGuid());
            Assert.AreEqual(userA.UserName, userB.UserName);
        }

        private static void AssertContains(IList<IChannel> channels, IChannel channel)
        {
            Assert.IsNotNull(channel);
            Assert.IsNotNull(channels);
            if (
                !channels.Any(
                    item =>
                        (item.Link.AbsoluteUri == channel.Link.AbsoluteUri)
                        && (item.Rss.AbsoluteUri == channel.Rss.AbsoluteUri) && (item.Title == channel.Title)))
            {
                Assert.Fail("Channel not found.");
            }
        }

        private static IChannel CreateTestChannel()
        {
            return new Channel(TestChannelGuid, TestChannelLink, TestChannelRss, TestChannelTitle);
        }

        private static IChannel CreateTestChannel2()
        {
            return new Channel(TestChannelGuid2, TestChannelLink2, TestChannelRss2, TestChannelTitle2);
        }

        private static IUser CreateTestUser()
        {
            return new User(TestUserGuid, TestUserName, string.Empty).SetPassword(TestPassword);
        }

        #endregion
    }
}
namespace Test.Model.Persisence.Interface
{
    using System;

    using global::Model.Persistence.Class;
    using global::Model.Persistence.Extension;
    using global::Model.Persistence.Interface;

    using NUnit.Framework;

    using Shared.Exceptions;

    // ReSharper disable InconsistentNaming
    public abstract class RssServiceTest
    {
        #region Constants

        private const string TestChannelTitle = "testChannelTitle";

        private const string TestItemDescription = "testItemDescription";

        private const int TestItemSequence = 7;

        private const string TestItemTitle = "testItemTitle";

        #endregion

        #region Static Fields

        private static readonly Guid TestChannelGuid = Guid.Parse("72B49E77-6782-49c7-A3CE-6D5EEDB9FA67");

        private static readonly Uri TestChannelLink = new Uri("http://test.com//channel.html");

        private static readonly Uri TestChannelRss = new Uri("http://test.com//channel.rss");

        private static readonly Guid TestItemGuid = Guid.Parse("2091C0BB-3AC9-4cb8-9060-D56E144A94FA");

        private static readonly Uri TestItemLink = new Uri("http://test.com//item.html");

        #endregion

        #region Fields

        private readonly IRssService rssService;

        #endregion

        #region Constructors and Destructors

        protected RssServiceTest(IRssService rssService)
        {
            this.rssService = rssService;
        }

        #endregion

        #region Public Methods and Operators

        [Test]
        [ExpectedException(typeof(NotFoundException))]
        public void AddItem_ChannelDoesNotExist_NotFoundException()
        {
            IItem item = CreateTestItem();
            this.rssService.AddItem(TestChannelGuid, item);
        }

        [Test]
        public void AddItem_Created_Successful()
        {
            bool existed;
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel);
            IItem item = CreateTestItem();
            this.rssService.AddItem(TestChannelGuid, item, out existed);
            Assert.IsFalse(existed);
        }

        [Test]
        public void AddItem_Updated_Successful()
        {
            bool existed;
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel);
            IItem item = CreateTestItem();
            this.rssService.AddItem(TestChannelGuid, item);
            this.rssService.AddItem(TestChannelGuid, item, out existed);
            Assert.IsTrue(existed);
        }

        [Test]
        public void DeleteChannel_Exists_Successful()
        {
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel);
            this.rssService.DeleteChannel(TestChannelGuid);
            IChannel getChannel = this.rssService.GetChannel(TestChannelGuid);
            Assert.IsNull(getChannel);
        }

        [Test]
        public void DeleteChannel_NotFound_Successful()
        {
            this.rssService.DeleteChannel(TestChannelGuid);
        }

        [Test]
        public void GetChannel_Exists_Successful()
        {
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel);
            IChannel getChannel = this.rssService.GetChannel(TestChannelGuid);
            AssertAreEqual(channel, getChannel);
        }

        [Test]
        public void GetChannel_NotFound_Successful()
        {
            IChannel getChannel = this.rssService.GetChannel(TestChannelGuid);
            Assert.IsNull(getChannel);
        }

        [Test]
        public void GetItem_Exists_Successful()
        {
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel);
            IItem item = CreateTestItem();
            this.rssService.AddItem(TestChannelGuid, item);
            IItem getItem = this.rssService.GetItem(TestItemGuid);
            AssertAreEqual(item, getItem);
        }

        [Test]
        public void GetItem_NotFound_Successful()
        {
            IItem getItem = this.rssService.GetItem(TestItemGuid);
            Assert.IsNull(getItem);
        }

        [Test]
        public void PutChannel_Created_Successful()
        {
            bool existed;
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel, out existed);
            Assert.IsFalse(existed);
        }

        [Test]
        public void PutChannel_Updated_Successful()
        {
            bool existed;
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel);
            this.rssService.PutChannel(channel, out existed);
            Assert.IsTrue(existed);
        }

        [Test]
        public void RemoveItem_Exists_Successful()
        {
            IChannel channel = CreateTestChannel();
            this.rssService.PutChannel(channel);
            IItem item = CreateTestItem();
            this.rssService.AddItem(TestChannelGuid, item);
            this.rssService.RemoveItem(TestItemGuid);
            IItem getItem = this.rssService.GetItem(TestItemGuid);
            Assert.IsNull(getItem);
        }

        [Test]
        public void RemoveItem_NotFound_Successful()
        {
            this.rssService.RemoveItem(TestItemGuid);
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            this.rssService.DeleteChannel(TestChannelGuid);
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

        private static void AssertAreEqual(IItem itemA, IItem itemB)
        {
            Assert.IsNotNull(itemA);
            Assert.IsNotNull(itemB);
            Assert.AreEqual(itemA.Description, itemB.Description);
            Assert.AreEqual(itemA.GetItemGuid(), itemB.GetItemGuid());
            Assert.AreEqual(itemA.Link.AbsoluteUri, itemB.Link.AbsoluteUri);
            Assert.AreEqual(itemA.Title, itemB.Title);
        }

        private static IChannel CreateTestChannel()
        {
            return new Channel(TestChannelGuid, TimeSpan.Zero, TestChannelLink, TestChannelRss, TestChannelTitle);
        }

        private static IItem CreateTestItem()
        {
            return new Item(TestItemGuid, TestItemDescription, TestItemLink, TestItemSequence, TestItemTitle);
        }

        #endregion
    }
}
namespace FeedReader.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using FeedReader.Class;

    using Model.Api.Interface;
    using Model.Persistence.Interface;

    [Authorize]
    [RoutePrefix("api/channel")]
    public class ChannelController : ApiController
    {
        #region Fields

        private readonly IChannelService channelService;

        #endregion

        #region Constructors and Destructors

        public ChannelController(IChannelService channelService)
        {
            this.channelService = channelService;
        }

        #endregion

        #region Public Methods and Operators

        [HttpDelete]
        [Route("{channelGuid}")]
        public void Delete(Guid channelGuid)
        {
            var identity = (Identity)this.User.Identity;
            this.channelService.RemoveChannel(identity.User.GetUserGuid(), channelGuid);
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<IChannel> Get()
        {
            var identity = (Identity)this.User.Identity;
            return this.channelService.EnumerateChannels(identity.User.GetUserGuid());
        }

        [HttpGet]
        [Route("{channelGuid}")]
        public IEnumerable<IUserItem> Get(
            Guid channelGuid,
            int limit = 0,
            bool before = false,
            Guid? itemGuid = default(Guid?))
        {
            var identity = (Identity)this.User.Identity;
            return this.channelService.EnumerateUserItems(
                identity.User.GetUserGuid(),
                channelGuid,
                limit,
                before,
                itemGuid);
        }

        [HttpPost]
        [Route("")]
        public IChannel Post(AddChannelDto addChannelDto)
        {
            var identity = (Identity)this.User.Identity;
            if (!addChannelDto.Rss.ToLower().StartsWith("http"))
            {
                addChannelDto.Rss = "http://" + addChannelDto.Rss;
            }

            return this.channelService.AddChannel(identity.User.GetUserGuid(), new Uri(addChannelDto.Rss));
        }

        #endregion

        public class AddChannelDto
        {
            #region Public Properties

            public string Rss { get; set; }

            #endregion
        }
    }
}
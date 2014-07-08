namespace Implementation.Api.Implementation
{
    using System;
    using System.Collections.Generic;

    using global::Implementation.Api.Class;

    using Model.Api.Interface;
    using Model.Persistence.Extension;
    using Model.Persistence.Interface;

    public class ChannelService : IChannelService
    {
        #region Fields

        private readonly IRssService rssService;

        private readonly IUserService userService;

        #endregion

        #region Constructors and Destructors

        public ChannelService(IRssService rssService, IUserService userService)
        {
            this.rssService = rssService;
            this.userService = userService;
        }

        #endregion

        #region Public Methods and Operators

        public IChannel AddChannel(Guid userGuid, Uri rss)
        {
            IChannel channel = RssHelper.Process(rss, this.rssService);
            this.userService.AddChannel(userGuid, channel);
            return channel;
        }

        public IEnumerable<IChannel> EnumerateChannels(Guid userGuid)
        {
            return this.userService.EnumerateChannels(userGuid);
        }

        public IEnumerable<IUserItem> EnumerateUserItemsAfter(
            Guid userGuid,
            Guid channelGuid,
            int limit,
            Guid? itemGuid)
        {
            IChannel channel = this.rssService.GetChannel(channelGuid);
            RssHelper.Process(channel.Rss, this.rssService);
            return this.userService.EnumerateUserItemsAfter(userGuid, channelGuid, limit, itemGuid);
        }

        public void RemoveChannel(Guid userGuid, Guid channelGuid)
        {
            this.userService.RemoveChannel(userGuid, channelGuid);
        }

        #endregion
    }
}
namespace Implementation.Api.Implementation
{
    using System;
    using System.Collections.Generic;

    using global::Implementation.Api.Class;

    using Model.Api.Interface;
    using Model.Persistence.Interface;

    public class ItemService : IItemService
    {
        #region Fields

        private readonly IRssService rssService;

        #endregion

        #region Constructors and Destructors

        public ItemService(IRssService rssService)
        {
            this.rssService = rssService;
        }

        #endregion

        #region Public Methods and Operators

        public IEnumerable<IItem> EnumerateItemsAfter(Guid channelGuid, int limit, Guid? itemGuid)
        {
            IChannel channel = this.rssService.GetChannel(channelGuid);
            RssHelper.Process(channel.Rss, this.rssService);
            return this.rssService.EnumerateItemsAfter(channelGuid, limit, itemGuid);
        }

        #endregion
    }
}
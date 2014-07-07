namespace Model.Api.Interface
{
    using System;
    using System.Collections.Generic;

    using Model.Persistence.Interface;

    public interface IChannelService
    {
        #region Public Methods and Operators

        IChannel AddChannel(Guid userGuid, Uri rss);

        IEnumerable<IChannel> EnumerateChannels(Guid userGuid);

        void RemoveChannel(Guid userGuid, Guid channelGuid);

        #endregion
    }
}
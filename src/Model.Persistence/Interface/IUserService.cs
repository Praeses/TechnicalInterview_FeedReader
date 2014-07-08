namespace Model.Persistence.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IUserService
    {
        #region Public Methods and Operators

        void AddChannel(Guid userGuid, IChannel channel, out bool existed);

        void AddToken(Guid userGuid, IToken token, out bool existed);

        void DeleteUser(Guid userGuid);

        IEnumerable<IChannel> EnumerateChannels(Guid userGuid);

        IEnumerable<IUserItem> EnumerateUserItemsAfter(Guid userGuid, Guid channelGuid, int limit, Guid? itemGuid);

        IUser GetUser(string userName);

        IUser GetUser(Guid tokenGuid, out IToken token);

        void PutUser(IUser user, out bool existed);

        void PutUserItem(Guid userGuid, Guid itemGuid, bool read, out bool existed);

        void RemoveChannel(Guid userGuid, Guid channelGuid);

        void RemoveToken(Guid tokenGuid);

        #endregion
    }
}
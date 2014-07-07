namespace Model.Persistence.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IRssService
    {
        #region Public Methods and Operators

        void AddItem(Guid channelGuid, IItem item, out bool existed);

        void DeleteChannel(Guid channelGuid);

        IEnumerable<IItem> EnumerateItemsAfter(Guid channelGuid, int limit, Guid? itemGuid);

        IChannel GetChannel(Guid channelGuid);

        IItem GetItem(Guid itemGuid);

        void PutChannel(IChannel channel, out bool existed);

        void RemoveItem(Guid itemGuid);

        #endregion
    }
}
namespace Model.Persistence.Interface
{
    using System;

    public interface IRssService
    {
        #region Public Methods and Operators

        void AddItem(Guid channelGuid, IItem item, out bool existed);

        void DeleteChannel(Guid channelGuid);

        IChannel GetChannel(Guid channelGuid);

        IItem GetItem(Guid itemGuid);

        void PutChannel(IChannel channel, out bool existed);

        void RemoveItem(Guid itemGuid);

        #endregion
    }
}
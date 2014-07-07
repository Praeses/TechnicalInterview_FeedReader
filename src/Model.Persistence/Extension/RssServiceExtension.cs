namespace Model.Persistence.Extension
{
    using System;

    using Model.Persistence.Interface;

    public static class RssServiceExtension
    {
        #region Public Methods and Operators

        public static void AddItem(this IRssService @this, Guid channelGuid, IItem item)
        {
            bool existed;
            @this.AddItem(channelGuid, item, out existed);
        }

        public static void PutChannel(this IRssService @this, IChannel channel)
        {
            bool existed;
            @this.PutChannel(channel, out existed);
        }

        #endregion
    }
}
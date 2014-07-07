namespace Model.Persistence.Interface
{
    using System;

    public interface IChannel
    {
        #region Public Properties

        Guid? ChannelGuid { get; set; }

        Uri Link { get; }

        Uri Rss { get; }

        string Title { get; }

        #endregion

        #region Public Methods and Operators

        Guid GetChannelGuid();

        #endregion
    }
}
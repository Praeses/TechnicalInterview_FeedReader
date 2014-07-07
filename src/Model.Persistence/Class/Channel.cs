namespace Model.Persistence.Class
{
    using System;

    using Model.Persistence.Interface;

    public class Channel : IChannel
    {
        #region Constructors and Destructors

        public Channel(Uri link, Uri rss, string title)
        {
            this.Link = link;
            this.Rss = rss;
            this.Title = title;
        }

        public Channel(Guid channelGuid, Uri link, Uri rss, string title)
        {
            this.ChannelGuid = channelGuid;
            this.Link = link;
            this.Rss = rss;
            this.Title = title;
        }

        #endregion

        #region Public Properties

        public Guid? ChannelGuid { get; set; }

        public Uri Link { get; private set; }

        public Uri Rss { get; private set; }

        public string Title { get; private set; }

        #endregion

        #region Public Methods and Operators

        public Guid GetChannelGuid()
        {
            if (this.ChannelGuid.HasValue)
            {
                return this.ChannelGuid.Value;
            }

            throw new NullReferenceException();
        }

        #endregion
    }
}
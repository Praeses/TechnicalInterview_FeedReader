using System;
using System.Collections.Generic;

namespace FeedReader.Domain
{
    public class Feed
    {
        private readonly IFeedReader _reader;

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }

        private ICollection<FeedItem> _feedItems;

        public ICollection<FeedItem> FeedItems
        {
            get
            {
                try
                {
                    if (_feedItems == null)
                    {
                        _feedItems = new List<FeedItem>();
                        _reader.LoadFeedItems(this);
                    }

                    return _feedItems;
                }
                catch (Exception)
                {
                    
                    throw new Exception("Unable to load feed Items for feed " + Name);
                }
                
            }
            set { _feedItems = value; }
        }

        public Feed()
        {
            _reader = new WebFeedReader();
        }

        public Feed(IFeedReader reader)
        {
            _reader = reader;
        }
    }
}

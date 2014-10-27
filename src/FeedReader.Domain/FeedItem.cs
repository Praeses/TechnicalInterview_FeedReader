using System;

namespace FeedReader.Domain
{
    public class FeedItem
    {
        
        public virtual int Id { get; set; }
        public virtual int FeedId { get; set; }
        public virtual string Source { get; set; }
        public virtual string Title { get; set; }
        public virtual string Link { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime? PublishDate { get; set; }
    }
}

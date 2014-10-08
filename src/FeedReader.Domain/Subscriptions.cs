namespace FeedReader.Domain
{
    public class Subscription
    {
        public int Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual int FeedId { get; set; }
    }
}

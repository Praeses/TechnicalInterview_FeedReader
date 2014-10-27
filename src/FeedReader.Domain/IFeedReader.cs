namespace FeedReader.Domain
{
    public interface IFeedReader 
    {
        bool LoadFeedItems(Feed f);
    }
}

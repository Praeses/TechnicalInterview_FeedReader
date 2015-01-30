using FeedReader.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Models.Interfaces
{
    public interface IFeedRepository
    {
        List<FeedUrl> GetFeedUrlsByUserId(string id);

        List<Feed> SearchFeeds(string id, string searchEntry);

        int AddFeed(Feed feed);

        void UpdateUserInfoFeeds(string userId, int feedId);

        void RemovedUserInfoFeeds(string userId, int feedId);
    }
}

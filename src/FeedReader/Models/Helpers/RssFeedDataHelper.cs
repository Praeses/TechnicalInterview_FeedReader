using FeedReader.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models.Helpers
{
    public class RssFeedDataHelper
    {
        public RssFeed retrieveRssFeedFromDb(string rssFeedUrl)
        {
            RssFeed existingRssFeed = null;

            using (var db = new FeedReaderContext())
            {
                existingRssFeed = db.RssFeeds.SingleOrDefault(a => a.Url == rssFeedUrl);          
            }

            return existingRssFeed;
        }

        public void saveRssFeed(RssFeed rssFeed)
        {
            using (var db = new FeedReaderContext())
            {
                db.RssFeeds.Add(rssFeed);
                db.SaveChanges();
            }
        }

        public void saveUserRssFeed(UserRssFeed userRssFeed)
        {
            using (var db = new FeedReaderContext())
            {
                db.UserRssFeeds.Add(userRssFeed);
                db.SaveChanges();
            }
        }

        public UserRssFeed retireveUserRssFeedFromDb(RssFeed rssFeed, ApplicationUser applicationUser)
        {
            UserRssFeed existingUserRssFeed = null;
            using (var db = new FeedReaderContext())
            {
                existingUserRssFeed = db.UserRssFeeds.SingleOrDefault(a => a.RssFeedId == rssFeed.RssFeedId &&
                                                                           a.UserId == applicationUser.Id
                                                                      );
            }

            return existingUserRssFeed;
        }
    }
}
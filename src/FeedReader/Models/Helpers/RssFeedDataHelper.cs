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

        public void updateRssFeedsForUser(string userId)
        {
            RssFeedService rssFeedService = new RssFeedService();

            //Get user feed urls and ids
            using (var db = new FeedReaderContext())
            {
                var rssFeedQuery = from rssFeedT in db.RssFeeds
                                       join userRssFeedsT in db.UserRssFeeds on rssFeedT.RssFeedId equals userRssFeedsT.RssFeedId
                                       where userRssFeedsT.UserId == userId
                                       select rssFeedT;

                //List of feeds we need to check for updates
                List<RssFeed> rssFeeds = rssFeedQuery.ToList();

                RssFeed newRssFeed = null;
                RssFeedItem checkRssFeedItem = null;

                foreach(RssFeed currentRssFeed in rssFeeds)
                {
                    newRssFeed = rssFeedService.RetrieveRssFeed(currentRssFeed.Url);

                    foreach(RssFeedItem newRssFeedItem in newRssFeed.RssFeedItems)
                    {
                        //Check to see if this one is in our database
                        checkRssFeedItem = db.RssFeedItems.FirstOrDefault(a => a.Title == newRssFeedItem.Title);

                        if (checkRssFeedItem == null)
                        {
                            //Set RssFeedId to have reference to existing RssFeed
                            newRssFeedItem.RssFeedId = currentRssFeed.RssFeedId;
                            db.RssFeedItems.Add(newRssFeedItem);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public List<RssFeedItem> retrieveRssFeedItemsForUser(string userId, string search, int page)
        {
            List<RssFeedItem> rssFeedItems = null;

            using (var db = new FeedReaderContext())
            {
                var rssFeedItemQuery = from rssFeedItemT in db.RssFeedItems
                                       join userRssFeedsT in db.UserRssFeeds on rssFeedItemT.RssFeedId equals userRssFeedsT.RssFeedId
                                       where userRssFeedsT.UserId == userId && (rssFeedItemT.Title.Contains(search) || rssFeedItemT.Description.Contains(search))
                                       orderby rssFeedItemT.PublishDate descending
                                       select rssFeedItemT;

                rssFeedItemQuery = rssFeedItemQuery.Skip((page - 1) * 15).Take(15);

                rssFeedItems = rssFeedItemQuery.ToList();
            }

            return rssFeedItems;
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
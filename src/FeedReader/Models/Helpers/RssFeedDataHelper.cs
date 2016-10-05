using FeedReader.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models.Helpers
{
    // Helper class manages all interactions with the database
    public class RssFeedDataHelper
    {   
        // Method retrieves a rss feed object given the url
        public RssFeed retrieveRssFeed(string rssFeedUrl)
        {
            RssFeed existingRssFeed = null;

            using (var db = new FeedReaderContext())
            {
                existingRssFeed = db.RssFeeds.SingleOrDefault(a => a.Url == rssFeedUrl);          
            }

            return existingRssFeed;
        }

        // Method updates all rss feeds tied to the user
        public void updateRssFeedsForUser(string userId)
        {
            RssFeedService rssFeedService = new RssFeedService();

            using (var db = new FeedReaderContext())
            {
                // Returns us the rss feeds for the user
                var rssFeedQuery = from rssFeedT in db.RssFeeds
                                       join userRssFeedsT in db.UserRssFeeds on rssFeedT.RssFeedId equals userRssFeedsT.RssFeedId
                                       where userRssFeedsT.UserId == userId
                                       select rssFeedT;

                // List of feeds we need to check for updates
                List<RssFeed> rssFeeds = rssFeedQuery.ToList();

                RssFeed newRssFeed = null;
                RssFeedItem checkRssFeedItem = null;

                // Loop through our rss feed
                foreach(RssFeed currentRssFeed in rssFeeds)
                {
                    // Get new rss feed from service
                    newRssFeed = rssFeedService.RetrieveRssFeed(currentRssFeed.Url);

                    // Loop through the possible new rss feed items
                    foreach(RssFeedItem newRssFeedItem in newRssFeed.RssFeedItems)
                    {
                        //Check to see if this one is in our database
                        checkRssFeedItem = db.RssFeedItems.FirstOrDefault(a => a.Title == newRssFeedItem.Title);

                        // If not go ahead and store this one
                        if (checkRssFeedItem == null)
                        {
                            // Set RssFeedId to have reference to existing RssFeed
                            newRssFeedItem.RssFeedId = currentRssFeed.RssFeedId;
                            db.RssFeedItems.Add(newRssFeedItem);
                        }
                        else
                        {
                            break; // Break out of loop if we found the first matching one
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        // Retrieves all rss feed items for a user, takes in search, filtering, and pagination
        public List<RssFeedItem> retrieveRssFeedItemsForUser(string userId, string search, string filter, int page)
        {
            List<RssFeedItem> rssFeedItems = null;

            using (var db = new FeedReaderContext())
            {
                // Query gets feed items for user and search criteria
                var rssFeedItemQuery = from rssFeedItemT in db.RssFeedItems
                                       join userRssFeedsT in db.UserRssFeeds on rssFeedItemT.RssFeedId equals userRssFeedsT.RssFeedId
                                       where userRssFeedsT.UserId == userId &&
                                             (rssFeedItemT.Title.Contains(search) || rssFeedItemT.Description.Contains(search))
                                       orderby rssFeedItemT.PublishDate descending
                                       select rssFeedItemT;

                // If we have a rss feed id to filter get just those values
                if (filter != "")
                {
                    rssFeedItemQuery = rssFeedItemQuery.Where(x => x.RssFeedId.ToString() == filter);
                }

                // Take pagination into account
                rssFeedItemQuery = rssFeedItemQuery.Skip((page - 1) * 15).Take(15);

                rssFeedItems = rssFeedItemQuery.ToList();
            }

            return rssFeedItems;
        }

        // Method saves the rss feed to the database
        public void saveRssFeed(RssFeed rssFeed)
        {
            using (var db = new FeedReaderContext())
            {
                db.RssFeeds.Add(rssFeed);
                db.SaveChanges();
            }
        }

        // Method saves the user rss feed to the database
        public void saveUserRssFeed(UserRssFeed userRssFeed)
        {
            using (var db = new FeedReaderContext())
            {
                db.UserRssFeeds.Add(userRssFeed);
                db.SaveChanges();
            }
        }

        // Method gets the user rss feed for given feed it and user id
        public UserRssFeed retireveUserRssFeed(int rssFeedId, string userId)
        {
            UserRssFeed existingUserRssFeed = null;
            using (var db = new FeedReaderContext())
            {
                existingUserRssFeed = db.UserRssFeeds.SingleOrDefault(a => a.RssFeedId == rssFeedId &&
                                                                           a.UserId == userId
                                                                      );
            }

            return existingUserRssFeed;
        }

        // Method retrieves all rss feeds for a user
        public List<RssFeed> retrieveRssFeedsForUser(string userId)
        {
            List<RssFeed> rssFeeds = null;
            using (var db = new FeedReaderContext())
            {
                var rssFeedQuery = from rssFeedT in db.RssFeeds
                                       join userRssFeedT in db.UserRssFeeds on rssFeedT.RssFeedId equals userRssFeedT.RssFeedId
                                       where userRssFeedT.UserId == userId
                                       orderby rssFeedT.Title
                                       select rssFeedT;

                rssFeeds = rssFeedQuery.ToList();
            }

            return rssFeeds;
        }

        // Method removes the user rss feed from the database
        public void removeUserRssFeed(string userId, string rssFeedId)
        {
            UserRssFeed existingUserRssFeed = null;
            using (var db = new FeedReaderContext())
            {
                existingUserRssFeed = db.UserRssFeeds.SingleOrDefault(a => a.RssFeedId.ToString() == rssFeedId &&
                                                                           a.UserId == userId
                                                                      );

                if (existingUserRssFeed != null)
                {
                    db.UserRssFeeds.Remove(existingUserRssFeed);
                    db.SaveChanges();
                }
            }
        }
    }
}
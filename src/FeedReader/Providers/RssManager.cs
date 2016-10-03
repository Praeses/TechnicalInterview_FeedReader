using FeedReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using FeedReader.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace FeedReader.Providers
{
    /// <summary>
    /// Acts as the layer between the Front end and the managing of Rss related Items
    /// </summary>
    public class RssManager
    {
        private RssContext dbContext;
        private string appUserId;

        /// <summary>
        /// Most all actions are done across a user in the RssContext. Include them here so they don't have to be passed into each method.
        /// </summary>
        /// <param name="userId"></param>
        public RssManager(string userId)
        {
            this.dbContext = new RssContext();
            this.appUserId = userId;
            //dbContext.Database.Log = s => Debug.WriteLine(s);
        }

        /// <summary>
        /// Finds a subscription tied to a particular user if it exists.
        /// </summary>
        /// <param name="url">Requested subscription feed url</param>
        /// <returns>Subscription if it exists or null</returns>
        public RssSubscription FindSubscription(string url)
        {
            var subs = dbContext.RssSubscriptions.Include("Feed").Where(a => a.Feed.FeedUrl == url && a.UserId == appUserId);

            RssSubscription subscription = subs.FirstOrDefault();
            
            return subscription;
        }

        /// <summary>
        /// Add a subscription to the user. Throws an RssUpdateException if the feed cannot be reached.
        /// </summary>
        /// <param name="feedUrl">RssChannel feed url</param>
        /// <returns>RssSubscription or RssUpdateException if the requested URL cannot be reached</returns>
        public RssSubscription AddSubscription(string feedUrl)
        {
            RssChannel retrievedChannel = FindChannel(feedUrl);

            RssSubscription subscription = null;
            ApplicationUser appUser = dbContext.Users.Find(appUserId);

            subscription = new RssSubscription();
            subscription.Feed = retrievedChannel;

            appUser.RssSubscriptions.Add(subscription);
            dbContext.SaveChanges();

            return subscription;

        }

        /// <summary>
        /// Finds all subscriptions for the user
        /// </summary>
        /// <returns>ICollection of RssSubscriptions with included RssChannel Feed</returns>
        public ICollection<RssSubscription> RetrieveSubscriptions()
        {
            ICollection<RssSubscription> subs = dbContext.RssSubscriptions.Where(sub => sub.UserId == appUserId).Include(sub => sub.Feed).Include("Feed").ToList();

            return subs;
        }

        /// <summary>
        /// Finds a RssChannel object represting the feedUrl passed in. If it exists in the database it will be return or a new one will be created and added to the database.
        /// </summary>
        /// <param name="feedUrl">Requested URL feed</param>
        /// <returns>RssChannel or RssUpdateExcepption if the feedurl could not be reached</returns>
        public RssChannel FindChannel(string feedUrl)
        {
            RssChannel channel = dbContext.RssChannels.Where(a => a.FeedUrl == feedUrl).FirstOrDefault();

            if (channel == null)
            {
                IRssUpdater updater = new ChainingRssReader();
                channel = updater.RetrieveChannel(feedUrl);

                channel = dbContext.RssChannels.Add(channel);
                dbContext.SaveChanges();
            }

            return channel;
        }

        /// <summary>
        /// Method to signal a update to the Feeds that the user is subscribed to.
        /// </summary>
        public void RequestChannelUpdate()
        {
            ICollection<RssSubscription> subscriptions = RetrieveSubscriptions();
            List<RssChannel> feeds = new List<RssChannel>();

            foreach (RssSubscription sub in subscriptions)
            {
                feeds.Add(sub.Feed);
            }

            //update each rssChannel asyncrounously but syncronously wait for the return
            ThreadedChannelUpdater updater = new ThreadedChannelUpdater();
            updater.UpdateFeeds(feeds);
        }

        /// <summary>
        /// Remove a subcription from the user's account
        /// </summary>
        /// <param name="rssChannelId">The channel to remove the subscription from</param>
        public void RemoveSubscription(int rssChannelId)
        {
            RssSubscription subscription = dbContext.RssSubscriptions.Where(a=> a.UserId == appUserId && a.RssChannelId == rssChannelId).FirstOrDefault();
            dbContext.RssSubscriptions.Remove(subscription);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Updates RssItem attributes when requested
        /// </summary>
        /// <param name="rssItemId">ID of the item to be updated</param>
        /// <param name="hide">Hide Attribute to be saved</param>
        /// <param name="read">Read Attribute to be saved</param>
        /// <returns>The added attribute object</returns>
        public UserRssAttributes UpdateRssItem(int rssItemId, bool hide, bool read){
            UserRssAttributes rssAttributes = dbContext.UserRssAttributes.Where(a => a.UserId == appUserId && a.RssItemId == rssItemId).FirstOrDefault();

            if (rssAttributes == null)
            {
                rssAttributes = new UserRssAttributes();
                rssAttributes.UserId = appUserId;
                rssAttributes.RssItemId = rssItemId;
                rssAttributes.Read = read;
                rssAttributes.Hidden = hide;

                dbContext.UserRssAttributes.Add(rssAttributes);

                dbContext.SaveChanges();
            }

            return rssAttributes;
        }

        /// <summary>
        /// Retrieve all feed items in a Datatable response format. Performs a query to retrieve just the items requested so a large feed list can be returned quickly
        /// </summary>
        /// <param name="dTableRequest">Datatable request presenting the requeste search params</param>
        /// <returns>DataTable response object with the data requested</returns>
        /// 
        public DTableResponse<Object> RetrieveDataTableRssItems(DTableRequest dTableRequest)
        {
            //find all the application rss Items without filtering with a left join on the user attributes because they might not exist
            var entriesFirst = from rssItem in dbContext.RssItems
                               join rssChannel in dbContext.RssChannels on rssItem.RssChannelId equals rssChannel.RssChannelId
                               join rssSubscription in dbContext.RssSubscriptions on rssChannel.RssChannelId equals rssSubscription.RssChannelId
                               join user in dbContext.Users on rssSubscription.UserId equals user.Id
                               join userRssAttributes in dbContext.UserRssAttributes on new { p1 = user.Id, p2 = rssItem.RssItemId } equals new { p1 = userRssAttributes.UserId, p2 = userRssAttributes.RssItemId } into gj
                               from userRssAttributes in gj.DefaultIfEmpty()
                               select new { rssItem, rssChannel, rssSubscription, user, userRssAttributes };

 
            //order the results by the publish date
            var entries = from entry in entriesFirst
                      where entry.rssSubscription.UserId == appUserId && (entry.userRssAttributes == null || !entry.userRssAttributes.Hidden)
                      orderby entry.rssItem.PubDate descending
                      select new
                      {
                          entry.rssItem,
                          entry.userRssAttributes,
                          filteredChannel = new
                          {
                              Title = entry.rssChannel.Title,
                              FeedUrl = entry.rssChannel.FeedUrl,
                              Link = entry.rssChannel.Link
                          }
                      };
                         
            //get the total count before filtering
            int totalCount = entries.Count();

            //assume that not filtering is done.
            int filteredCount = totalCount;

            //if the user requested filtereing add it to the query. 
            //The search value is searched across the 
            //  Rss Feed Item Title, Rss Feed Item Description, and the RssChannel title
            string search = dTableRequest.search.value;
            if (search != null)
            {
                entries = entries.Where(a => a.rssItem.Title.Contains(search) || a.rssItem.Description.Contains(search) || a.filteredChannel.Title.Contains(search));

                filteredCount = entries.Count();
            }

            //return the page that the user requsted
            var entrySlice = entries.Skip(dTableRequest.start).Take(dTableRequest.length).ToList();

            //remove the html from the response to prevent xss and other bad things from happening.

            foreach (var entryItem in entrySlice)
            {
                entryItem.rssItem.Description = FeedReaderUtils.ScrubHtml(entryItem.rssItem.Description);
            }
            //include a custom object for datatables response as we can't have circular references and datatables expects things in a particular order
            //a separate object was considered but having it present on the return will allow easy view and updates into the filtering and structure that must occur in the response
            IEnumerable<Object> data = entrySlice.Select(e => new
            {
                wrapper = new {
                    title = e.rssItem.Title,
                    pubDate = e.rssItem.PubDate,
                    link = e.rssItem.Link,
                    rssItemId = e.rssItem.RssItemId,
                    description = (e.rssItem.Description.Length > 200) ? e.rssItem.Description.Substring(0, 199) + "..." : e.rssItem.Description,
                    read = e.userRssAttributes != null ? e.userRssAttributes.Read : false,
                    channel = new
                    {
                        feedUrl = e.filteredChannel.FeedUrl,
                        title = e.filteredChannel.Title,
                        link = e.filteredChannel.Link
                    }
                }
               
            }).ToList();

            DTableResponse<Object> dTableResponse = new DTableResponse<Object>(new Collection<Object>(data.ToList()));
            dTableResponse.recordsFiltered = filteredCount;
            dTableResponse.recordsTotal = totalCount;
            dTableResponse.draw = dTableRequest.draw;

            return dTableResponse;
        }
    }

    /// <summary>
    /// A multithreaded updater for multiple channel feeds.
    /// </summary>
    public class ThreadedChannelUpdater
    {
        /// <summary>
        /// Driver object for the task pool
        /// </summary>
        /// <param name="channels">List of channels to be updated</param>
        public void UpdateFeeds(ICollection<RssChannel> channels)
        {
            Task[] tasks= Enumerable.Range(0, channels.Count()).Select(i=>
               Task.Run(() =>
               {
                   Updater updater = new Updater(channels.ElementAt(i));
                   updater.UpdateChannel(null);
               })
            ).ToArray();

            //wait syncrounously for all the updates to complete
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Class that performs the work in the thread for channel updates
        /// </summary>
        private class Updater
        {
            private RssChannel channel;

            /// <summary>
            /// Constructor that takes in the channel to be updated
            /// </summary>
            /// <param name="channel">Requested channel to update</param>
            public Updater(RssChannel channel)
            {
                this.channel = channel;
            }

            /// <summary>
            /// Thread worker to update the channel
            /// </summary>
            /// <param name="threadContext">unused</param>
            public void UpdateChannel(Object threadContext)
            {
                RssContext dbContext = new RssContext();

                IRssUpdater updater = new ChainingRssReader();
                try
                {
                    channel = dbContext.RssChannels.Where(a => a.RssChannelId == channel.RssChannelId).FirstOrDefault();//replace with one in this context
                    byte[] rowVersion = channel.RowVersion;
                    RssChannel updatedChannel = updater.RetrieveChannel(channel.FeedUrl);
                    updatedChannel.RssChannelId = channel.RssChannelId;

                    dbContext.Entry(channel).CurrentValues.SetValues(updatedChannel);
                    dbContext.Entry(channel).OriginalValues["RowVersion"] = rowVersion;

                    ICollection<RssItem> itemsToBeAdded = new List<RssItem>();
                    foreach (RssItem updatedItem in updatedChannel.Items)
                    {
                        updatedItem.RssChannelId = channel.RssChannelId;//must set this on the new items for the hash. 
                        //Will think of a better approach so that we don't have to keep setting things to see if they are added
                        RssItem existingItem = dbContext.RssItems.Where(a => a.Hash == updatedItem.Hash).FirstOrDefault();
                        if (existingItem == null)
                        {
                            itemsToBeAdded.Add(updatedItem);
                        }
                        else
                        {
                            //update existing or leave?
                            //dbContext.Entry(existingItem).CurrentValues.SetValues(updatedItem);
                        }
                    }
                    foreach (RssItem itemToBeAdded in itemsToBeAdded)
                    {
                        channel.Items.Add(itemToBeAdded);
                    }

                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        Debug.WriteLine("RssChannel Update was attempted on an updating channel. Skip as it has been updated");
                    }
                }
                catch (RssUpdateException)
                {
                    Debug.WriteLine("Unable to reach the requested feed");
                }
            }
        }
    }
}
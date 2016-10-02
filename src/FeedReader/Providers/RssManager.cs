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

namespace FeedReader.Providers
{
    public class RssManager
    {
        private RssContext dbContext;
        private string appUserId;

        public RssManager(string userId)
        {
            this.dbContext = new RssContext();
            this.appUserId = userId;
            //dbContext.Database.Log = s => Debug.WriteLine(s);
        }

        public RssSubscription AddSubscription(string feedUrl)
        {
            RssChannel retrievedChannel = FindChannel(feedUrl);

            RssSubscription subscription = null;
            try
            {
                ApplicationUser appUser = dbContext.Users.Find(appUserId);

                subscription = new RssSubscription();
                subscription.Feed = retrievedChannel;

                appUser.RssSubscriptions.Add(subscription);
                dbContext.SaveChanges();

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return subscription;

        }
        public ICollection<RssSubscription> RetrieveSubscriptions()
        {
            ICollection<RssSubscription> subs = dbContext.RssSubscriptions.Where(sub => sub.UserId == appUserId).Include(sub => sub.Feed).Include("Feed.Items").ToList();

            return subs;
        }

        public RssChannel FindChannel(string feedUrl)
        {
            RssChannel channel = dbContext.RssChannels.Where(a => a.FeedUrl == feedUrl).FirstOrDefault();

            if (channel == null)
            {
                IRssUpdater updater = new XDocumentRssReader();
                channel = updater.RetrieveChannel(feedUrl);
            }

            return channel;
        }

        public void RequestChannelUpdate()
        {
            ICollection<RssSubscription> subscriptions = RetrieveSubscriptions();
            List<RssChannel> feeds = new List<RssChannel>();

            foreach (RssSubscription sub in subscriptions)
            {
                feeds.Add(sub.Feed);
            }

            IRssUpdater updater = new XDocumentRssReader();

            foreach (RssChannel channel in feeds)
            {
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
                catch (DbUpdateConcurrencyException ex)
                {
                    Debug.WriteLine("RssChannel Update was attempted on an updating channel. Skip as it has been updated");
                }
            }
        }

        public void RemoveSubscription(int rssChannelId)
        {
            RssSubscription subscription = dbContext.RssSubscriptions.Where(a=> a.UserId == appUserId && a.RssChannelId == rssChannelId).FirstOrDefault();
            dbContext.RssSubscriptions.Remove(subscription);
            dbContext.SaveChanges();
        }

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
        public DTableResponse<Object> RetrieveDataTableRssItems(DTableRequest dTableRequest)
        {
            var entriesFirst = from rssItem in dbContext.RssItems
                               join rssChannel in dbContext.RssChannels on rssItem.RssChannelId equals rssChannel.RssChannelId
                               join rssSubscription in dbContext.RssSubscriptions on rssChannel.RssChannelId equals rssSubscription.RssChannelId
                               join user in dbContext.Users on rssSubscription.UserId equals user.Id
                               join userRssAttributes in dbContext.UserRssAttributes on new { p1 = user.Id, p2 = rssItem.RssItemId } equals new { p1 = userRssAttributes.UserId, p2 = userRssAttributes.RssItemId } into gj
                               from userRssAttributes in gj.DefaultIfEmpty()
                               select new { rssItem, rssChannel, rssSubscription, user, userRssAttributes };

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
                           
            int totalCount = entries.Count();

            int filteredCount = totalCount;

            string search = dTableRequest.search.value;
            if (search != null)
            {
                entries = entries.Where(a => a.rssItem.Title.Contains(search) || a.rssItem.Description.Contains(search));

                filteredCount = entries.Count();
            }

            var entrySlice = entries.Skip(dTableRequest.start).Take(dTableRequest.length).ToList();

            foreach (var entryItem in entrySlice)
            {
                entryItem.rssItem.Description = FeedReaderUtils.ScrubHtml(entryItem.rssItem.Description);
            }
            //include a custom object for datatables response as we can't have circular references and datatables expects things in a particular order
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
}
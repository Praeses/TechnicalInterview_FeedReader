using FeedReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace FeedReader.Providers
{
    public class RssProvider
    {
        private RssContext dbContext;
        public RssProvider(RssContext dbContext)
        {
            this.dbContext = dbContext;
            //dbContext.Database.Log = s => Debug.WriteLine(s);
        }
        public ICollection<RssSubscription> retrieveSubscriptions(string userId)
        {
            ICollection<RssSubscription> subs = dbContext.RssSubscriptions.Where(sub => sub.UserId == userId).Include(sub => sub.Feed).Include("Feed.Items").ToList();

            return subs;
        }

        public DTableResponse<Object> retrieveDataTableRssItems(string userId, DTableRequest dTableRequest)
        {
            var entries = dbContext.RssItems
                .Join(
                dbContext.RssChannels,
                rssItem => rssItem.RssChannelId,
                rssChannel => rssChannel.RssChannelId,
                (rssItem, rssChannel) => new { rssItem, rssChannel })
                .Join(
                    dbContext.RssSubscriptions,
                    combinedEntry => combinedEntry.rssChannel.RssChannelId,
                    rssSubscription => rssSubscription.RssChannelId,
                    (combinedEntry, rssSubscription) => new
                    {
                        rssChannel = combinedEntry.rssChannel,
                        rssItem = combinedEntry.rssItem,
                        rssSubscription = rssSubscription
                    })
                .Join(
                    dbContext.Users,
                    combined => combined.rssSubscription.UserId,
                    user => user.Id,
                    (combinedEntry, user) => new
                    {
                        rssChannel = combinedEntry.rssChannel,
                        rssItem = combinedEntry.rssItem,
                        rssSubscription = combinedEntry.rssSubscription,
                        user = user
                    })
                  .Where(fullEntry => fullEntry.user.Id == userId)
                  .OrderBy(fullEntry => fullEntry.rssItem.PubDate)
                  .Select(result => new
                  {
                      result.rssItem,
                      filteredChannel = new
                      {
                          Title = result.rssChannel.Title,
                          FeedUrl = result.rssChannel.FeedUrl,
                          Link = result.rssChannel.Link
                      },
                  });

            int totalCount = entries.Count();

            int filteredCount = totalCount;

            string search = dTableRequest.search.value;
            if (search != null)
            {
                entries = entries.Where(a => a.rssItem.Title.Contains(search) || a.rssItem.Description.Contains(search));

                filteredCount = entries.Count();
            }

            var entrySlice = entries.Skip(dTableRequest.start).Take(dTableRequest.length).ToList();

            //include a custom object for datatables response as we can't have circular references and datatables expects things in a particular order
            IEnumerable<Object> data = entrySlice.Select(e => new
            {
                wrapper = new {
                    title = e.rssItem.Title,
                    pubDate = e.rssItem.PubDate,
                    link = e.rssItem.Link,
                    rssItemId = e.rssItem.RssItemId,
                    description = (e.rssItem.Description.Length > 100) ? e.rssItem.Description.Substring(0, 99) + "..." : e.rssItem.Description,
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
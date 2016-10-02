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
            var entriesFirst = from rssItem in dbContext.RssItems
                               join rssChannel in dbContext.RssChannels on rssItem.RssChannelId equals rssChannel.RssChannelId
                               join rssSubscription in dbContext.RssSubscriptions on rssChannel.RssChannelId equals rssSubscription.RssChannelId
                               join user in dbContext.Users on rssSubscription.UserId equals user.Id
                               join userRssAttributes in dbContext.UserRssAttributes on new { p1 = user.Id, p2 = rssItem.RssItemId } equals new { p1 = userRssAttributes.UserId, p2 = userRssAttributes.RssItemId } into gj
                               from userRssAttributes in gj.DefaultIfEmpty()
                               select new { rssItem, rssChannel, rssSubscription, user, userRssAttributes };

            var entries = from entry in entriesFirst
                      where entry.user.Id == userId
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

            //include a custom object for datatables response as we can't have circular references and datatables expects things in a particular order
            IEnumerable<Object> data = entrySlice.Select(e => new
            {
                wrapper = new {
                    title = e.rssItem.Title,
                    pubDate = e.rssItem.PubDate,
                    link = e.rssItem.Link,
                    rssItemId = e.rssItem.RssItemId,
                    description = (e.rssItem.Description.Length > 100) ? e.rssItem.Description.Substring(0, 99) + "..." : e.rssItem.Description,
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FeedService.Contract.ContentService;
using FeedService.Model;

namespace FeedService.DataAccess
{
    internal class FeedHelper
    {
        public async Task DownloadFeeds(ContentEntities context, IEnumerable<SubscriptionSynchronization> subscriptions)
        {
            IEnumerable<Task<List<SubscriptionItem>>> downloadTasksQuery =
                from feed in subscriptions select ProcessFeed(feed);

            List<Task<List<SubscriptionItem>>> downloadTasks = downloadTasksQuery.ToList();

            while (downloadTasks.Count > 0)
            {
                Task<List<SubscriptionItem>> finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask);

                // Await the completed task. 
                List<SubscriptionItem> items = await finishedTask;
                items.ForEach(tmp => context.SubscriptionItems.Add(tmp));
            }
        }
        Task<List<SubscriptionItem>> ProcessFeed(SubscriptionSynchronization feed)
        {
            return Task<List<SubscriptionItem>>.Factory.StartNew(() =>
            {
                var expiration = DateTime.UtcNow.AddDays(feed.ItemRetentionInDays);
                var retList = new List<SubscriptionItem>();
                var client = new WebClient();
                client.Headers.Add("user-agent", "myNewsstand/1.0");
                var resourceStream = client.OpenRead(new Uri(feed.ResourceUri));
                if (resourceStream != null)
                {
                    var xmlReader = XmlReader.Create(resourceStream);
                    var loadedFeed = SyndicationFeed.Load(xmlReader);
                    if (loadedFeed != null)
                    {
                        foreach (var item in loadedFeed.Items)
                        {
                            if (feed.CurrentItems.Contains(item.Id))
                                continue;
                            var feedBuilder = new StringBuilder();
                            using (var feedWriter = XmlWriter.Create(feedBuilder))
                            {
                                var formatter = item.GetAtom10Formatter();
                                formatter.WriteTo(feedWriter);
                                feedWriter.Close();
                            }
                            var returnItem = new SubscriptionItem
                            {
                                SubscriptionId = feed.SubscriptionId,
                                ItemId = item.Id,
                                ExpirationDateUtc = expiration,
                                Content = feedBuilder.ToString(),
                                PublishedDateUtc = item.PublishDate.UtcDateTime
                            };
                            retList.Add(returnItem);
                        }
                    }
                }
                return retList;
            });
        }
    }
}
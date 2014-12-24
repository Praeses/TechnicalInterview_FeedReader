using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FeedService.Contract.ContentService;
using FeedService.Contract.SubscriptionService;
using FeedService.Model;

namespace FeedService.DataAccess
{
    internal class FeedHelper
    {
        public Subscription BuildSubscription(NewSubscription subscriptionRequest)
        {
            //lookup resource
            var webClient = new WebClient();
            webClient.Headers.Add("user-agent", "myNewsstand/1.0");
            var xmlReader = new RssXmlReader(webClient.OpenRead(subscriptionRequest.ResourceUri));
            var feed = SyndicationFeed.Load(xmlReader);
            if (feed != null)
            {
                feed.Items = new List<SyndicationItem>();
                var feedBuilder = new StringBuilder();
                using (var feedWriter = XmlWriter.Create(feedBuilder))
                {
                    var formatter = feed.GetAtom10Formatter();
                    formatter.WriteTo(feedWriter);
                    feedWriter.Close();
                }

                return new Subscription
                    {
                        AccountId = subscriptionRequest.AccountId,
                        ResourceUri = subscriptionRequest.ResourceUri,
                        Name = subscriptionRequest.Name,
                        ItemRetentionInDays = 30,
                        StartDateUtc = DateTime.UtcNow,
                        ContentType = "NEWSFEED",
                        LastRefreshedUtc = new DateTime(1900, 1, 1),
                        Summary = feedBuilder.ToString()
                    };
            }
            throw new Exception("Feed not found at specified uri");
        }
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
                    var xmlReader = new RssXmlReader(resourceStream);
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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedReader.Models
{
    public class ManageSubscriptionsModel
    {
        public ManageSubscriptionsModel()
        {
            Subscriptions = new ObservableCollection<SubscriptionListItem>();
        }
        public ObservableCollection<SubscriptionListItem> Subscriptions { get; set; }
        public NewSubscriptionModel NewItem { get; set; }
    }
    public class RemoveSubscriptionModel
    {
        public int SubscriptionId { get; set; }
    }
    public class NewSubscriptionModel
    {
        [Required]
        [StringLength(70, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Url]
        public string Uri { get; set; }
    }
    public class SubscriptionListItem
    {
        private SyndicationFeed _feed;
        public string Name { get; set; }
        public string Uri { get; set; }
        public int SubscriptionId { get; set; }
        public string Summary { get; set; }
        protected SyndicationFeed Feed
        {
            get
            {
                if (_feed == null && !string.IsNullOrWhiteSpace(Summary))
                {
                    using (var sr = new StringReader(Summary))
                    {
                        using (var xr = XmlReader.Create(sr))
                        {
                            _feed = SyndicationFeed.Load(xr);
                            xr.Close();
                        }
                    }
                }
                return _feed;
            }
        }
        public string ImageUrl 
        {
            get
            {
                return Feed != null && Feed.ImageUrl != null ? Feed.ImageUrl.ToString() : string.Empty;
            }
        }
        public string Description
        {
            get
            {
                if (Feed != null && Feed.Description != null)
                    return Feed.Description.Text;
                return string.Empty;
            }
        }
    }
}
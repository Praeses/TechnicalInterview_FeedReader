using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedReader.Models
{
    public class FeedModel
    {
        public FeedModel()
        {
            MenuItems = new List<MenuItem>();
        }
        public List<MenuItem> MenuItems { get; set; }
        public FeedItem SelectedItem { get; set; }
        public DetailOptions DetailOptions { get; set; }
    }
    public enum ViewMode
    {
        All = 0,
        Subscription = 1
    }
    public class DetailOptions
    {
        public ViewMode Mode { get; set; }
        public int SubscriptionId { get; set; }
        public string SearchPattern { get; set; }
        public int LastSubscriptionItemId { get; set; }
        public List<FeedItem> DisplayItems { get; set; }
    }
    public class MenuItem
    {
        public string Name { get; set; }
        public int SubscriptionId { get; set; }
    }
    public class FeedItem
    {
        private SyndicationItem _item;
        public int SubscriptionItemId { get; set; }
        public string Publisher { get; set; }
        public string PublishedContent { get; set; }
        protected SyndicationItem PublishedItem
        {
            get
            {
                if (_item == null && !string.IsNullOrWhiteSpace(PublishedContent))
                {
                    using (var sr = new StringReader(PublishedContent))
                    {
                        var formatter = new Atom10ItemFormatter();
                        using (var xr = XmlReader.Create(sr))
                        {
                            formatter.ReadFrom(xr);
                            _item = formatter.Item;
                        }
                    }
                }
                return _item;
            }
        }
        public string Title
        {
            get { return PublishedItem != null ? PublishedItem.Title.Text : string.Empty;
            }
        }
        public string Summary
        {
            get { return PublishedItem != null && PublishedItem.Summary != null ? WebUtility.HtmlDecode(PublishedItem.Summary.Text) : string.Empty; }
        }
        public string Content
        {
            get { return PublishedItem != null ? PublishedItem.Content.ToString() : string.Empty; }
        }
        public string Link
        {
            get
            {
                return PublishedItem != null
                           && PublishedItem.Links.Count > 0 ? PublishedItem.Links[0].Uri.ToString() : string.Empty;
            }
        }
    }

}
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedReader.Models
{
    public class FeedModel
    {
        public FeedModel()
        {
            MenuItems = new List<MenuItem>();
            DisplayItems = new List<FeedItem>();
        }
        public List<MenuItem> MenuItems { get; set; }
        public List<FeedItem> DisplayItems { get; set; }
        public FeedItem SelectedItem { get; set; }
    }
    public class DetailOptions
    {
        public int? SubscriptionId { get; set; }
        public string SearchString { get; set; }
    }
    public class DetailModel
    {
        public List<FeedItem> DisplayItems { get; set; }
        public FeedItem SelectedItem { get; set; }
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
            get { return PublishedItem != null ? PublishedItem.Summary.Text : string.Empty; }
        }
        public string Content
        {
            get { return PublishedItem != null ? PublishedItem.Content.ToString() : string.Empty; }
        }
    }

}
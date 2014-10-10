using System;
using System.Data.Entity;

namespace FeedReader.Models
{
    public class FeedItem : IComparable<FeedItem>
    {
        public string Title { get; set; }
        public string address { get; set; }
        public string siteTitle { get; set; }
        public string baseUri { get; set; }
        public DateTimeOffset published {get; set;}

        public int CompareTo(FeedItem other)
        {
            return this.published.CompareTo(other.published);
        }
    }


}
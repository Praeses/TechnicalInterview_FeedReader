using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Models.Views
{
    public class FeedItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Guid { get; set; }
        public DateTime PubDate { get; set; }
        public List<string> MediaSrc { get; set; }

        public FeedItem()
        {
            MediaSrc = new List<string>();
        }
    }
}

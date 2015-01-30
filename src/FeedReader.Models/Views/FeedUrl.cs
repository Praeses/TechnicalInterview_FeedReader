using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Models.Views
{
    public class FeedUrl
    {
        public int FeedID { get; set; }
        public string Url { get; set; }
        public string SiteUrl { get; set; }
        public int Subscribers { get; set; }
    }
}

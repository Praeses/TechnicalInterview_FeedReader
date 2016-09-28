﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class RssChannel
    {
        public RssChannel(){
            Items = new List<RssItem>();
        }

        public int RssChannelId { get; set; }
        public string FeedUrl { get; set; }

        //required fields
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        public List<RssItem> Items { get; set; }

        //optional
        public string ImageUrl { get; set; }
        public string LastBuildDate { get; set; }
        public string LastPubDate { get; set; }
        public string Ttl { get; set; }
        public string Language { get; set; }
        public string CopyRight { get; set; }
        public string ManagingEditor { get; set; }

    }
}
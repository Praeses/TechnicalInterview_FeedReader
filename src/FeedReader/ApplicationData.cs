using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FeedReader.Models;

namespace FeedReader
{
    public static class ApplicationData
    {
        public static List<NewsFeed> AllNewsFeeds 
        {
            get
            {
                if (HttpContext.Current.Application[Constants.Application.ALL_NEWS_FEEDS] == null)
                {
                    HttpContext.Current.Application[Constants.Application.ALL_NEWS_FEEDS] = new List<NewsFeed>();
                }

                return (List<NewsFeed>)HttpContext.Current.Application[Constants.Application.ALL_NEWS_FEEDS];
            }

            set { HttpContext.Current.Application[Constants.Application.ALL_NEWS_FEEDS] = value; }
        }
    }
}
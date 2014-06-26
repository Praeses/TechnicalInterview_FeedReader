using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace FeedReader.Models
{
    public class RssFeedReader
    {
         Dictionary<string, List<RssArticle>> rssMap = new Dictionary<string, List<RssArticle>>();

         public Dictionary<string, List<RssArticle>> ReadSubscribedFeeds(string UserId)
         {
             List<RssFeed> subscribedFeeds = LoadSubscribedFeeds(UserId);

             foreach(RssFeed feed in subscribedFeeds){
                 List<RssArticle> album = ReadFeedForChannel(feed.Link);
                 rssMap.Add(feed.Title, album);
             }

             return rssMap;
         }


         private List<RssArticle> ReadFeedForChannel(string Url)
         {
             List<RssArticle> album = new List<RssArticle>();

             XDocument feedXml = XDocument.Load(Url);

             var articles = from feed in feedXml.Descendants("item")
                         select new RssArticle
                         {
                             Title = feed.Element("title").Value,
                             Link = feed.Element("link").Value,
                             Description = feed.Element("description").Value,                             
                             PublicationDate = feed.Element("pubDate").Value
                         };


             foreach (RssArticle article in articles)
             {
                 album.Add(article);
             }

             return album;
         }

         private List<RssFeed> LoadSubscribedFeeds(string UserId)
         {
             List<RssFeed> subbedFeeds = new List<RssFeed>();

             ApplicationDbContext db = new ApplicationDbContext();

             //Write the query to be used
             string query = "SELECT * FROM RssFeeds WHERE UserId = @UserId";

             //Set the parameters
             SqlParameter userParam = new SqlParameter("UserId", UserId);
             object[] parameters = new object[] { userParam };

             IEnumerable<RssFeed> feeds = db.RssFeeds.SqlQuery(query, parameters);

             foreach(RssFeed feed in feeds){
                 subbedFeeds.Add(feed);
             }

             return subbedFeeds;
         }
         
    }

    
}
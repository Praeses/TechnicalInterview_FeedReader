using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace FeedReader.Models
{
    public class RssFeedReader
    {          
         public static Dictionary<string, List<RssArticle>> ReadSubscribedFeeds(string UserId)
         {
             Dictionary<string, List<RssArticle>> rssMap = new Dictionary<string, List<RssArticle>>();
             List<RssFeed> subscribedFeeds = LoadSubscribedFeeds(UserId);

             foreach(RssFeed feed in subscribedFeeds){
                 List<RssArticle> album = ReadFeedForChannel(feed.Link);
                 rssMap.Add(feed.Title, album);
             }

             return rssMap;
         }


         private static List<RssArticle> ReadFeedForChannel(string Url)
         {
             List<RssArticle> album = new List<RssArticle>();
             IEnumerable<RssArticle> articles = album;//using the album just to initialize here

             try
             {
                 XDocument feedXml = XDocument.Load(Url);

                 articles = from feed in feedXml.Descendants("item")
                            select new RssArticle
                            {
                                Title = feed.Element("title").Value,
                                Link = feed.Element("link").Value,
                                Description = StripTagsRegexCompiled(feed.Element("description").Value),
                                PublicationDate = feed.Element("pubDate").Value
                            };
             }
             catch(Exception e)
             {
                 //Don't add anything to the album.
             }

             album.AddRange(articles);
             
             return album;
         }

         public static List<RssFeed> LoadSubscribedFeeds(string UserId)
         {
             ApplicationDbContext db = new ApplicationDbContext();
             var subbedFeeds = db.RssFeeds.Where(x => x.UserId == UserId);
             return subbedFeeds.ToList();             
         }

         private static string StripTagsRegexCompiled(string source)
         {
             Regex htmlRegex = new Regex("(?i)<.*?>", RegexOptions.Compiled);
             Regex bbCodeRegex = new Regex(Regex.Escape("[") + ".*?]", RegexOptions.Compiled);
             
             source = bbCodeRegex.Replace(source, string.Empty);//replace [bbcode] elements
             source = htmlRegex.Replace(source, string.Empty);//replace html tags

             return source;
         }

        public static bool validateRssLink(string Url){
            bool isUrlValid = false;

            try
            {
                 XDocument feedXml = XDocument.Load(Url);
                 isUrlValid = true;
            }
            catch(Exception e)
            { }

            return isUrlValid;
        }
         
    }

    
}
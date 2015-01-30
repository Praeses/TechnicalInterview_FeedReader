using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using FeedReader.Models.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FeedReader.Models.Views;
using FeedReader.UI.Extenstions;

namespace FeedReader.UI.Controllers
{
    public class FeedController : Controller
    {
        private IFeedRepository _feedRepository;
        private IXmlParser _xmlHelper;

        public FeedController(IFeedRepository feedRepository, IXmlParser xmlHelper)
        {
            _feedRepository = feedRepository;
            _xmlHelper = xmlHelper;
        }

        // GET: Feed
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var feedUrls = _feedRepository.GetFeedUrlsByUserId(User.Identity.GetUserId());
            var urls = feedUrls.GetUrlFromFeedUrl();
            var feeds = new List<Feed>();

            var xmlFeeds = await LoadXML(urls);

            foreach (var result in xmlFeeds)
            {
                var feed = _xmlHelper.CreateFeedFromXml(result);
                feed.FeedID = feedUrls.Where(f => f.SiteUrl == feed.Url).Select(f => f.FeedID).First();
                feed.Subscribers = feedUrls.Where(f => f.SiteUrl == feed.Url).Select(f => f.Subscribers).First();
                feeds.Add(feed);
            }

            return View(feeds);
        }

        // GET: Feed/Search 
        [Authorize]
        public async Task<ActionResult> Search(string searchEntry)
        {
            var results = new List<Feed>();
            results = _feedRepository.SearchFeeds(User.Identity.GetUserId(), searchEntry);
            if (!results.Any())
            {
                HttpClient client = new HttpClient();
                var rss = await LoadFeedAsync(searchEntry, client);
                if(rss == null)
                {
                    ViewBag.Message = "We couldn't find any feeds matching your entry, try another search.";
                    return View(new List<Feed>());
                }
                var feed = _xmlHelper.CreateFeedFromXml(rss);
                
                //Add feed to database if it exists even if the user does not subscribe
                //to avoid having to reload when someone else enters the rssUrl in the search
                feed.RssUrl = searchEntry;
                feed.FeedID = _feedRepository.AddFeed(feed);
                results.Add(feed);
            }
            return View(results);
        }

        #region XML Loading Helpers
        private async Task<List<XDocument>> LoadXML(List<string> feedUrls)
        {
            HttpClient client = new HttpClient();
            var feeds = new List<XDocument>();

            var downloadTasksQuery = from url in feedUrls select LoadFeedAsync(url, client);
            List<Task<XDocument>> downloadTasks = downloadTasksQuery.ToList();

            while (downloadTasks.Count > 0)
            {
                Task<XDocument> firstFinished = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(firstFinished);
                var result = await firstFinished;
                feeds.Add(result);
            }
            return feeds;
        }

        private async Task<XDocument> LoadFeedAsync(string url, HttpClient client)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string urlString = await response.Content.ReadAsStringAsync();

                var rss = XDocument.Parse(urlString);
                return rss;
            }
            catch (Exception ex)
            {
                //Log the excepction message somewhere and return null - will check for return value
                //equal to null to determine if an error message needs to be shown to the user.
            }
            return null;
        }
        #endregion
    }
}
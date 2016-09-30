using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FeedReader.Providers
{
    interface IRssSearchProvider
    {
        string search(string query);
    }

    public class FeedlyRssProvider : IRssSearchProvider
    {
        private string apiKey;
        private string feedlyBaseUrl;
        
        public FeedlyRssProvider()
        {
            apiKey = System.Configuration.ConfigurationManager.AppSettings["FeedlyApiKey"];
            feedlyBaseUrl = System.Configuration.ConfigurationManager.AppSettings["FeedlyBaseUrl"];
        }
        public string search(string query)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Oauth: " + apiKey);
            string jsonResponse = client.DownloadString(feedlyBaseUrl + "/" + "v3/search/feeds" + "?" + "query=" + HttpUtility.UrlEncode(query));

            return jsonResponse;
        }
    }
}

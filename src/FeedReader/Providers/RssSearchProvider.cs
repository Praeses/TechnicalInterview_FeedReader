using FeedReader.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace FeedReader.Providers
{
    interface IRssSearchProvider
    {
        RssSearchResponse Search(string query);
    }

    public class RssSearchResponse
    {
        public RssSearchResponse(){
            this.results = new List<SearchResultEntry>();
        }
        public string query { get; set; }
        public ICollection<SearchResultEntry> results { get; set; }

        public class SearchResultEntry
        {
            public string url { get; set; }
            public string title { get; set; }
            public string contentSnippet { get; set; }
            public string link { get; set; }
        }
    }

    public class GoogleRssProvider : IRssSearchProvider
    {
        private string _baseUrl;

        public GoogleRssProvider()
        {
            this._baseUrl = System.Configuration.ConfigurationManager.AppSettings["GoogleFeedApiLink"];
        }

        private RssSearchResponse ParseResponse(string jsonResult)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            GoogleResponse serviceResponse = serializer.Deserialize<GoogleResponse>(jsonResult);

            RssSearchResponse response = new RssSearchResponse();

            response.query = serviceResponse.responseData.query;
           
            foreach(FeedReader.Providers.GoogleRssProvider.GoogleResponse.GoogleEntry serviceEntry in serviceResponse.responseData.entries){
                FeedReader.Providers.RssSearchResponse.SearchResultEntry entry = new FeedReader.Providers.RssSearchResponse.SearchResultEntry();

                entry.contentSnippet = serviceEntry.contentSnippet;
                entry.link = serviceEntry.link;
                entry.title = serviceEntry.title;
                entry.url = serviceEntry.url;
                
                response.results.Add(entry);
            }

            return response;
        }

        public RssSearchResponse Search(string query)
        {
            WebClient client = new WebClient();
            string jsonResponse = client.DownloadString(_baseUrl + "/" + "find" + "?" + "v=1.0&q=" + HttpUtility.UrlEncode(query));

            RssSearchResponse response = ParseResponse(jsonResponse);

            return response;
        }

        public class GoogleResponse
        {
            public InnerResponse responseData {get; set;}

            public class InnerResponse {
                public string query {get; set;}
                public ICollection<GoogleEntry> entries {get; set;}
            }

            public class GoogleEntry {
                public string url {get; set;}
                public string title {get; set;}
                public string contentSnippet {get; set;}
                public string link {get; set;}
            }
        }
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

        private RssSearchResponse parseResponse(string jsonResponse){

            return null;
        }
        public RssSearchResponse Search(string query)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Oauth: " + apiKey);
            string jsonResponse = client.DownloadString(feedlyBaseUrl + "/" + "v3/search/feeds" + "?" + "query=" + HttpUtility.UrlEncode(query));

            RssSearchResponse response = parseResponse(jsonResponse);

            return response;
        }
    }
}

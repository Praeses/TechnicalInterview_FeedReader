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
    /// <summary>
    /// Interface for providing search results to clients based on a string query
    /// </summary>
    interface IRssSearchProvider
    {
        RssSearchResponse Search(string query);
    }

    /// <summary>
    /// Object container an rss search result set
    /// </summary>
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

    /// <summary>
    /// Rss Search provider using googles feed api
    /// </summary>
    public class GoogleRssProvider : IRssSearchProvider
    {
        private string _baseUrl;

        /// <summary>
        /// Initialize the class with the base link from the settings
        /// </summary>
        public GoogleRssProvider()
        {
            this._baseUrl = System.Configuration.ConfigurationManager.AppSettings["GoogleFeedApiLink"];
        }

        /// <summary>
        /// Parse the google response into a response the client understands
        /// </summary>
        /// <param name="jsonResult">JSON data from the service called</param>
        /// <returns>Parsed RssSearchResponse </returns>
        private RssSearchResponse ParseResponse(string jsonResult)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            GoogleResponse serviceResponse = serializer.Deserialize<GoogleResponse>(jsonResult);

            RssSearchResponse response = new RssSearchResponse();

            response.query = serviceResponse.responseData.query;
           
            // map each of google's objects to our own
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

        /// <summary>
        /// Provide an RssSearch response based on a string query
        /// </summary>
        /// <param name="query">String to be searched</param>
        /// <returns>RssSearchResponse of query results</returns>
        public RssSearchResponse Search(string query)
        {
            WebClient client = new WebClient();
            string jsonResponse = client.DownloadString(_baseUrl + "/" + "find" + "?" + "v=1.0&q=" + HttpUtility.UrlEncode(query));

            RssSearchResponse response = ParseResponse(jsonResponse);

            return response;
        }

        /// <summary>
        /// Class representing the JSON object that returns from google's feed api
        /// </summary>
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

    /// <summary>
    /// A class that uses the Feedly api to provide search results. Due to the api restrictions of this service, I elected to not use it to parse
    /// </summary>
    public class FeedlyRssProvider : IRssSearchProvider
    {
        private string apiKey;
        private string feedlyBaseUrl;
        
        /// <summary>
        /// Constructor that grabs the base url and api key from the configs
        /// </summary>
        public FeedlyRssProvider()
        {
            apiKey = System.Configuration.ConfigurationManager.AppSettings["FeedlyApiKey"];
            feedlyBaseUrl = System.Configuration.ConfigurationManager.AppSettings["FeedlyBaseUrl"];
        }


        /// <summary>
        /// Unimplemeted method to parse a response from feedly
        /// </summary>
        /// <param name="jsonResponse"></param>
        /// <returns></returns>
        private RssSearchResponse parseResponse(string jsonResponse){

            return null;
        }

        /// <summary>
        /// Provide an RssSearch response based on a string query
        /// </summary>
        /// <param name="query">String to be searched</param>
        /// <returns>RssSearchResponse of query results</returns>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace FeedReader.Utils
{

    /// <summary>
    /// Extension of WebClient that allows the setting of a timeout
    /// </summary>
    public class RssWebClient : WebClient
    {
        private int _timeout;

        /// <summary>
        /// Retrieves the timeout from the app settings
        /// </summary>
        public RssWebClient() : base()
        {
            _timeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RssRetrieverTimeoutMillis"]);
        }

        /// <summary>
        /// Overrides the base method to set timeout
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest w =  base.GetWebRequest(address);
            w.Timeout = _timeout;
            return w;
        }
    }
}
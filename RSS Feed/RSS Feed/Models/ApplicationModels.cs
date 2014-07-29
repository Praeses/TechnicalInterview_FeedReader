using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;

namespace RSS_Feed.Models
{
    public class ApplicationViewModel
    {
        List<SyndicationFeed> feedList = new List<SyndicationFeed>();
    }
}
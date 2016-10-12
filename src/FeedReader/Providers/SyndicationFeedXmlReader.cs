using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Globalization;
using System.Diagnostics;
using System.IO;

namespace FeedReader.Providers
{
    //T

    /// <summary>
    /// There are some feeds that do not conform to the date standard for RSS that are not handled by the standard xmlReader. Ex. http://feeds.foxnews.com/foxnews/latest 
    /// This has been pulled and reviewed from the stackoverflow post http://stackoverflow.com/questions/210375/problems-reading-rss-with-c-sharp-and-net-3-5 and https://gist.github.com/jaminto/495843
    /// as well as the official msdna site describing the issue.
    /// </summary>
    public class SyndicationFeedXmlReader : XmlTextReader
    {
        readonly string[] Rss20DateTimeHints = { "pubDate" };
        readonly string[] Atom10DateTimeHints = { "updated", "published", "lastBuildDate" };
        private bool isRss2DateTime = false;
        private bool isAtomDateTime = false;
        static DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;

        public SyndicationFeedXmlReader(Stream stream) : base(stream) {
           
        }

        public SyndicationFeedXmlReader(string inputUri) : base(inputUri) { }

        public override bool IsStartElement(string localname, string ns)
        {
            isRss2DateTime = false;
            isAtomDateTime = false;

            if (Rss20DateTimeHints.Contains(localname)) isRss2DateTime = true;
            if (Atom10DateTimeHints.Contains(localname)) isAtomDateTime = true;

            return base.IsStartElement(localname, ns);
        }

        public override string ReadString()
        {
            Debug.WriteLine("Read using custom syndication reader");
            string dateVal = base.ReadString();

            try
            {
                if (isRss2DateTime)
                {
                    MethodInfo objMethod = typeof(Rss20FeedFormatter).GetMethod("DateFromString",
                                                                                 BindingFlags.NonPublic |
                                                                                 BindingFlags.Static);
                    Debug.Assert(objMethod != null);
                    objMethod.Invoke(null, new object[] { dateVal, this });

                }
                if (isAtomDateTime)
                {
                    MethodInfo objMethod = typeof(Atom10FeedFormatter).GetMethod("DateFromString",
                                                                                  BindingFlags.NonPublic |
                                                                                  BindingFlags.Instance);
                    Debug.Assert(objMethod != null);
                    objMethod.Invoke(new Atom10FeedFormatter(), new object[] { dateVal, this });
                }
            }
            catch (TargetInvocationException)
            {
                try
                {
                    return DateTime.Parse(dateVal).ToString(dtfi.RFC1123Pattern);
                }
                catch (FormatException)
                {
                    return DateTimeOffset.UtcNow.ToString(dtfi.RFC1123Pattern);
                }
            }
            return dateVal;
        }
    }
}
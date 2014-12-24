using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace FeedService.DataAccess
{
    /// <summary>
    /// Custom Xml reader for RSS streams using non-standard date schemes
    /// </summary>
    class RssXmlReader : XmlTextReader
    {
        private bool _readingDate;
        const string CustomUtcDateTimeFormat = "ddd MMM dd HH:mm:ss Z yyyy"; 
        public RssXmlReader(Stream s) : base(s) { }
        public RssXmlReader(string inputUri) : base(inputUri) { }
        public override void ReadStartElement()
        {
            if (string.Equals(base.NamespaceURI, string.Empty, StringComparison.InvariantCultureIgnoreCase) &&
                (string.Equals(base.LocalName, "lastBuildDate", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(base.LocalName, "pubDate", StringComparison.InvariantCultureIgnoreCase)))
            {
                _readingDate = true;
            }
            base.ReadStartElement();
        }
        public override void ReadEndElement()
        {
            if (_readingDate)
            {
                _readingDate = false;
            }
            base.ReadEndElement();
        }
        public override string ReadString()
        {
            if (_readingDate)
            {
                string dateString = base.ReadString();
                DateTime dt;
                if (!DateTime.TryParse(dateString, out dt))
                    dt = DateTime.ParseExact(dateString, CustomUtcDateTimeFormat, CultureInfo.InvariantCulture);
                return dt.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
            }
            return base.ReadString();
        }
    }
}
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace FeedReader.Domain
{
    public class Feed 
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }

        public XDocument GetFeed()
        {
            try
            {   
                var xmlDoc = XDocument.Load(Url);

                // Add source!
                foreach (var item in xmlDoc.Descendants("item"))
                    item.Add(new XElement("source", Name));

                return xmlDoc;
            }
            catch (Exception)
            {
                return new XDocument();
            }
        }
    }
}

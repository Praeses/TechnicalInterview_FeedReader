namespace Implementation.Api.Class
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Model.Persistence.Class;
    using Model.Persistence.Extension;
    using Model.Persistence.Interface;

    using Shared.Exceptions;
    using Shared.Extension;

    public static class RssHelper
    {
        #region Public Methods and Operators

        public static IChannel Process(Uri rss, IRssService rssService)
        {
            try
            {
                XDocument rssXDocument = XDocument.Load(rss.AbsoluteUri);
                XElement channelElement = rssXDocument.XPathSelectElement("/rss/channel");

                string link = GetText(channelElement, "link[1]");
                string title = GetText(channelElement, "title[1]");

                IChannel channel = new Channel(new Uri(link), rss, title);
                rssService.PutChannel(channel);

                var items = new List<IItem>();
                foreach (XElement itemElement in channelElement.Descendants("item"))
                {
                    string description = GetText(itemElement, "description[1]");
                    link = GetText(itemElement, "link[1]");
                    title = GetText(itemElement, "title[1]");

                    IItem item = new Item(description, new Uri(link), title);
                    if (items.Exists(i => i.Link.AbsoluteUri == link))
                    {
                        continue;
                    }

                    items.Add(item);
                }

                items.Reverse();
                foreach (IItem item in items)
                {
                    rssService.AddItem(channel.GetChannelGuid(), item);
                }

                return channel;
            }
            catch (WebException e)
            {
                var res = (HttpWebResponse)e.Response;
                Type exceptionType = (res != default(HttpWebResponse))
                    ? res.StatusCode.ToExceptionType()
                    : typeof(BadRequestException);
                var exception = (Exception)Activator.CreateInstance(exceptionType, "Could not access url.", e);
                exception.AddDumpObject(() => rss);
                throw exception;
            }
            catch (XmlException e)
            {
                throw new BadRequestException("Url is not an RSS feed.", e);
            }
        }

        #endregion

        #region Methods

        private static string GetText(XElement xElement, string xPath)
        {
            xElement = xElement.XPathSelectElement(xPath);
            return xElement == default(XElement) ? null : xElement.Value;
        }

        #endregion
    }
}
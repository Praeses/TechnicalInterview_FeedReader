using FeedReader.Models;
using FeedReader.Providers;
using FeedReader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class Class1
    {
        //[Fact]
        //public void TestLocalFile()
        //{
        //    //arrange
        //    var updater = new ChainingRssReader();

        //    string url = ("verge.xml");

        //    //act
        //    RssChannel channel = updater.RetrieveChannel(url);

        //    Assert.NotNull(channel);
        //}

        //[Fact]
        //public void TestInternetFile()
        //{
        //    var updater = new ChainingRssReader();

        //    string url = "http://www.theverge.com/rss/full.xml";

        //    RssChannel channel = updater.RetrieveChannel(url);

        //    Assert.NotNull(channel);
        //}

        [Fact]
        public void TestScrubHtml()
        {
            var htmlString = "<script>alert('hi')</script>";

            var scrubbedHtml = FeedReaderUtils.ScrubHtml(htmlString);

            Assert.Equal(scrubbedHtml, "alert('hi')");
        }

        [Fact]
        public void TestScrubHtmlInnerOpeningTag()
        {
            var htmlString = "<script><alert('hi')</script>";

            var scrubbedHtml = FeedReaderUtils.ScrubHtml(htmlString);

            Assert.Equal("<alert('hi')", scrubbedHtml);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedReader.Models;

namespace FeedReader.Providers
{
    public interface IRssUpdater
    {
        RssChannel retrieveChannel(String url);
    }
}

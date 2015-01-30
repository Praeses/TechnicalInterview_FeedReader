using FeedReader.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace FeedReader.UI.Controllers
{
    public class FeedApiController : ApiController
    {
        private IFeedRepository _feedRepository;

        public FeedApiController(IFeedRepository feedRepository)
        {
            _feedRepository = feedRepository;
        }

        [HttpPost]
        public HttpResponseMessage Post(int id)
        {
            _feedRepository.UpdateUserInfoFeeds(User.Identity.GetUserId(), id);
            var response = Request.CreateResponse(HttpStatusCode.Accepted, id);
            return response;
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            _feedRepository.RemovedUserInfoFeeds(User.Identity.GetUserId(), id);
            var response = Request.CreateResponse(HttpStatusCode.Accepted, id);
            return response;
        }
    }
}

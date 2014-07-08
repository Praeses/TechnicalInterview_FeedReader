namespace FeedReader.ApiControllers
{
    using System;
    using System.Web.Http;

    using FeedReader.Class;

    using Model.Api.Interface;

    [Authorize]
    [RoutePrefix("api/userItem")]
    public class UserItemController : ApiController
    {
        #region Fields

        private readonly IUserItemService userItemService;

        #endregion

        #region Constructors and Destructors

        public UserItemController(IUserItemService userItemService)
        {
            this.userItemService = userItemService;
        }

        #endregion

        #region Public Methods and Operators

        [HttpPut]
        [Route("{itemGuid}")]
        public void Put(Guid itemGuid, SetUserItemDto setUserItemDto)
        {
            var identity = (Identity)this.User.Identity;
            this.userItemService.PutUserItem(identity.User.GetUserGuid(), itemGuid, setUserItemDto.Read);
        }

        #endregion

        public class SetUserItemDto
        {
            #region Public Properties

            public bool Read { get; set; }

            #endregion
        }
    }
}
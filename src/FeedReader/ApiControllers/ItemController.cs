namespace FeedReader.ApiControllers
{
    using System.Web.Http;

    using Model.Api.Interface;

    [Authorize]
    [RoutePrefix("api/item")]
    public class ItemController : ApiController
    {
        #region Fields

        private readonly IItemService itemService;

        #endregion

        #region Constructors and Destructors

        public ItemController(IItemService itemService)
        {
            this.itemService = itemService;
        }

        #endregion
    }
}
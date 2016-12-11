namespace FeedReader.Controllers
{
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        #region Public Methods and Operators

        public ActionResult Index()
        {
            return this.View();
        }

        #endregion
    }
}
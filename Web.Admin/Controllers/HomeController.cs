using System.Web.Mvc;

namespace Web.Admin.Controllers
{
    public class HomeController : RootController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
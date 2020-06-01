using System.Web.Mvc;

namespace Web.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class HomeController : RootController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
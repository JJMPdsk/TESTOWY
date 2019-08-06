using System.Web.Mvc;

namespace Auth.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Strona główna";

            return View();
        }
    }
}
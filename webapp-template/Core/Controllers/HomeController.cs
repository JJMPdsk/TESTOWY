using System.Web.Mvc;

namespace Core.Controllers
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
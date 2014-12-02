using System.Web.Mvc;

namespace RGA.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Ciekawe wyzwanie, rozwiązujące praktyczny problem.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Autorzy.";

            return View();
        }
    }
}
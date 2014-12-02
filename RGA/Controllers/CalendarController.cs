using System.Web.Mvc;

namespace RGA.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        [Authorize(Roles = "Kierowca")]
        public ActionResult Index()
        {
            return View();
        }

        //<div id="datepicker"></div>

        [Authorize(Roles = "Pracownik")]
        public ActionResult Courier(string id)
        {
            return View();
        }
    }
}
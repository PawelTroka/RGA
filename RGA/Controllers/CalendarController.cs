using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Models;
using RGA.Models.ViewModels;

namespace RGA.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        [Authorize(Roles = "Kierowca")]
        public ActionResult Index()
        {


            var model2 = new CalendarViewModel();


            ApplicationDbContext db = ApplicationDbContext.Create();

            var store = new UserStore<User>(db);
            var userManager = new UserManager<User>(store);
            var user = userManager.FindByName(System.Web.HttpContext.Current.User.Identity.Name);

            model2.User = user;



            var allRoutes = db.Routes.ToList();

            var thisDriverRoutes = allRoutes.FindAll(r => r.Driver.Id == model2.User.Id);

            model2.WorkDatesForUser = "";

            var sb = new StringBuilder();

            foreach (var driverRoute in thisDriverRoutes)
            {
                sb.AppendFormat(@" ""{0}"",", driverRoute.StartDateTime.ToShortDateString());
            }
            sb.Remove(sb.Length - 1, 1);

            model2.WorkDatesForUser = sb.ToString();

            return View(model2);
        }

        //<div id="datepicker"></div>


    }
}
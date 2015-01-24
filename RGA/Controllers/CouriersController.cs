using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Models;
using RGA.Models.ViewModels;

namespace RGA.Controllers
{
    public class CouriersController : Controller
    {
        private readonly CouriersViewModel model = new CouriersViewModel();

        // GET: Couriers
        [Authorize(Roles = "Pracownik")]
        public ActionResult Index()
        {
            return View(model);
        }

        //new{User.Identity.GetUserId<string>()}
        [Authorize(Roles = "Pracownik")]
        public ActionResult Calendar(string id)
        {
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);

            var model2 = new CourierCalendarViewModel();


            model2.SelectedDriver = userManager.FindById(id);


            ApplicationDbContext dbContext = ApplicationDbContext.Create();

            List<Route> allRoutes = dbContext.Routes.ToList();

            List<Route> thisDriverRoutes = allRoutes.FindAll(r => r.Driver.Id == model2.SelectedDriver.Id);

            model2.WorkDatesForDriver = "";

            var sb = new StringBuilder();

            foreach (Route driverRoute in thisDriverRoutes)
            {
                sb.AppendFormat(@" ""{0}"",", driverRoute.StartDateTime.ToShortDateString());
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            model2.WorkDatesForDriver = sb.ToString();

            return View(model2);
        }
    }
}
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
            model.SelectedDriver = userManager.FindByIdAsync(id).Result;

            return View(model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Models;
using RGA.Models.ViewModels;

namespace RGA.Controllers
{
    public class CouriersController : Controller
    {
        private CouriersViewModel model = new CouriersViewModel();

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
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            model.SelectedDriver = userManager.FindByIdAsync(id).Result;

            return View(model);
        }
    }
}
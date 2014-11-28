using System;
using System.Web.Mvc;
using Google.Maps.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Models;
using RGA.Models.ViewModels;

namespace RGA.Controllers
{
    public class RoutesController : Controller
    {
        private RoutesViewModel model = new RoutesViewModel();
        
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View("Index", model);
        }

        [Authorize(Roles = "Pracownik")]
        [Authorize(Roles = "Admin")]
        public ActionResult GenerateRoute(string id, DateTime now)
        {
            model.Date = now;
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);
            model.Driver = userManager.FindByIdAsync(id).Result;
            model.Worker = userManager.FindByIdAsync(User.Identity.GetUserId()).Result;

            return View("GenerateRoute", model);
        }

        [Authorize(Roles = "Pracownik")]
        [Authorize(Roles = "Admin")]
        public ActionResult Route(string id)
        {
            return View("Route", model);
        }
    }
}
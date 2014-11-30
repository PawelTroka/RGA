using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Security;
using Google.Maps.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Models;
using RGA.Models.ViewModels;

namespace RGA.Controllers
{
    public class RoutesController : Controller
    {        
        private RoutesViewModel routesViewModel=new RoutesViewModel();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View("Index", routesViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult GenerateRoute(string id, DateTime date)
        {
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);

            var model = new GenerateRouteViewModel(User.Identity.GetUserId())
            {
                StartDate = date,
                DriverName = userManager.FindById(id).UserName,
                WorkerId = User.Identity.GetUserId()
            };

            return View("GenerateRoute", model);
        }


        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult AddAddress()
        {
            var shipment = new Shipment() {Id = Guid.NewGuid().ToString()};

            return PartialView("EditorTemplates/Shipment", shipment);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult GenerateRoute(RoutesViewModel model)
        {

            string id = "1";
            return RedirectToAction("Route",id);
        }


        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult Route(string id)
        {
            return View("Route", routesViewModel);
        }
    }
}
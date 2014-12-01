using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Security;
using Google.Maps.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Helpers;
using RGA.Models;
using RGA.Models.ViewModels;
using WebGrease.Css.Extensions;

namespace RGA.Controllers
{
    public class RoutesController : Controller
    {
        private RoutesViewModel routesViewModel = new RoutesViewModel();
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
            var shipment = new Shipment() { Id = Guid.NewGuid().ToString() };

            return PartialView("EditorTemplates/Shipment", shipment);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult GenerateRoute(GenerateRouteViewModel model)
        {
            var db = ApplicationDbContext.Create();

            var store = new UserStore<User>(db);
            var userManager = new UserManager<User>(store);

            var creator = userManager.FindById(model.WorkerId);

            var driver = userManager.FindByName(model.DriverName);

            var shipments = new List<Shipment>();
            model.Shipments.ForEach(s => shipments.Add(new Shipment(){Id = Guid.NewGuid().ToString(),DestinationAddress = s.DestinationAddress,Number = s.Number}));

            var route =  new Route
            {
                Id = Guid.NewGuid().ToString(),
                StartAddress = model.StartAddress,
                Shipments = shipments,
                Description = model.Description,
                Notes = new List<Note>(),
                RouteOptimizationAlgorithm = model.RouteOptimizationAlgorithm,
                RouteOptimizationProvider = model.RouteOptimizationProvider,
                RouteOptimizationType = model.RouteOptimizationType,
                StartDateTime = model.StartDate,
                State = RouteState.New,
                Worker = creator,
                Driver = driver
            };
            if (!string.IsNullOrEmpty(model.Note))
                route.Notes.Add(new Note() { Id=Guid.NewGuid().ToString(), Content = model.Note, DateAdded = DateTime.Now, Creator = creator, Driver = driver });


            var generator = new RouteGenerator(route);
            route = generator.GenerateRoute();
            

            db.Routes.Add(route);
            db.SaveChanges();

            return RedirectToAction("Route", "Routes", new {id=route.Id});
        }


        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult Route(string id)
        {
            var db = ApplicationDbContext.Create();
            var route = db.Routes.Find(id);

            return View("Route", route);
        }
    }
}
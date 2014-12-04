using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Helpers;
using RGA.Helpers.RouteGeneration;
using RGA.Models;
using RGA.Models.ViewModels;
using WebGrease.Css.Extensions;

namespace RGA.Controllers
{
    public class RoutesController : Controller
    {
        private readonly RoutesViewModel routesViewModel = new RoutesViewModel();

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
            var shipment = new Shipment { Id = Guid.NewGuid().ToString() };

            return PartialView("EditorTemplates/Shipment", shipment);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult GenerateRoute(GenerateRouteViewModel model)
        {
            if (model.Shipments.Count > 8 && model.RouteOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                ModelState.AddModelError("RouteOptimizationProvider", "Wybrany dostawca optymalizacji trasy (GoogleMaps) przyjmuje maksymalnie 8 adresów. Zmniejsz liczbę adresów lub wybierz innego dostawcę.");

            if (model.Shipments.Count > 25 && model.RouteOptimizationProvider == RouteOptimizationProvider.MapQuest)
                ModelState.AddModelError("RouteOptimizationProvider", "Wybrany dostawca optymalizacji trasy (MapQuest) przyjmuje maksymalnie 25 adresów. Zmniejsz liczbę adresów lub wybierz innego dostawcę.");

            if (model.Shipments.Count > 25 && model.DistanceMatrixProvider == DistanceMatrixProvider.MapQuest)
                ModelState.AddModelError("DistanceMatrixProvider", "Wybrany dostawca macierzy odległości (MapQuest) obsługuje maksymalnie 25 adresów. Zmniejsz liczbę adresów lub wybierz innego dostawcę.");


            for (int i = 0; i < model.Shipments.Count; i++)
            {
                if (string.IsNullOrEmpty(model.Shipments[i].DestinationAddress))
                    ModelState.AddModelError("Shipments[" + i.ToString() + "].DestinationAddress", "Pole adres nie może być puste");


                if (string.IsNullOrEmpty(model.Shipments[i].Number))
                    ModelState.AddModelError("Shipments[" + i.ToString() + "].Number", "Pole numer nie może być puste");


            }

            if (model.StartDate.Date < DateTime.Now.Date)
                ModelState.AddModelError("StartDate", "Data nie może być wartością w przeszłości");

            ApplicationDbContext db = ApplicationDbContext.Create();

            var store = new UserStore<User>(db);
            var userManager = new UserManager<User>(store);

            User creator = userManager.FindById(model.WorkerId);

            User driver = userManager.FindByName(model.DriverName);


            if (creator == null)
                ModelState.AddModelError("WorkerId", "Nieznany pracownik");

            if (driver == null)
                ModelState.AddModelError("DriverName", "Nieznany kierowca");

            if (ModelState.IsValid)
            {


                var shipments = new List<Shipment>();
                model.Shipments.ForEach(
                    s =>
                        shipments.Add(new Shipment
                        {
                            Id = Guid.NewGuid().ToString(),
                            DestinationAddress = s.DestinationAddress,
                            Number = s.Number
                        }));

                var route = new Route
                {
                    Id = Guid.NewGuid().ToString(),
                    StartAddress = model.StartAddress,
                    Shipments = shipments,
                    Description = model.Description,
                    Notes = new List<Note>(),
                    DistanceMatrixProvider = model.DistanceMatrixProvider,
                    RouteOptimizationAlgorithm = model.RouteOptimizationAlgorithm,
                    RouteOptimizationProvider = model.RouteOptimizationProvider,
                    RouteOptimizationType = model.RouteOptimizationType,
                    StartDateTime = model.StartDate,
                    State = RouteState.New,
                    Worker = creator,
                    Driver = driver
                };
                if (!string.IsNullOrEmpty(model.Note))
                    route.Notes.Add(new Note
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = model.Note,
                        DateAdded = DateTime.Now,
                        Creator = creator,
                        Driver = driver
                    });


                var generator = new RouteGenerator(route);

                try
                {
                    route = generator.GenerateRoute();
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "Wystąpił błąd!\n" + exception.ToString() + ((exception.InnerException != null) ? "\n" + exception.InnerException : ""));
                    model.init();
                    return View(model);
                }

                db.Routes.Add(route);


                try
                {
                    db.SaveChanges();
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "Nie udało się zapisać wygenerowanej trasy do bazy!\nPowód: " + exception.ToString() + ((exception.InnerException != null) ? "\n" + exception.InnerException.ToString() : ""));
                    model.init();
                    return View(model);
                }
                return RedirectToAction("Route", "Routes", new { id = route.Id });
            }
            model.init();
            return View(model);
        }


        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult Route(string id)
        {
            ApplicationDbContext db = ApplicationDbContext.Create();
            Route route = db.Routes.Find(id);

            return View("Route", route);
        }
    }
}
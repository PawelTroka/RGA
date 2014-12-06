using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
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
        private ApplicationDbContext db = ApplicationDbContext.Create();
        

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View("Index", routesViewModel);
        }


        // GET: Routes12/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Route route = db.Routes.Find(id);
            if (route == null)
            {
                return HttpNotFound();
            }

            var model = new EditRouteViewModel(route);
            return View(model);
        }

        // POST: Routes12/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditRouteViewModel model)
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

            //if (model.StartDate.Date < DateTime.Now.Date)
               // ModelState.AddModelError("StartDate", "Data nie może być wartością w przeszłości");

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
                var isNeededToGenerateAgain = false;

                var shipments = new List<Shipment>();
                model.Shipments.ForEach(
                    s =>
                        shipments.Add(new Shipment
                        {
                            Id = Guid.NewGuid().ToString(),
                            DestinationAddress = s.DestinationAddress,
                            Number = s.Number
                        }));



                var route = db.Routes.Find(model.Id);


                route.Description = model.Description;
                
                route.StartDateTime = model.StartDate;
                route.Worker = creator;
                route.Driver = driver;
                route.State = model.State;


                if (route.Notes == null)
                    route.Notes = new List<Note>();

                 if(!string.IsNullOrEmpty(model.Note) )
                    route.Notes.Add(new Note() { Creator = creator, DateAdded = DateTime.Now, Id = Guid.NewGuid().ToString(), Content = model.Note, Driver = driver });
                


                if (route.StartAddress != model.StartAddress)
                {
                    route.StartAddress = model.StartAddress;
                    isNeededToGenerateAgain = true;
                }

                if (route.DistanceMatrixProvider != model.DistanceMatrixProvider)
                {
                    route.DistanceMatrixProvider = model.DistanceMatrixProvider;
                    isNeededToGenerateAgain = true;
                }

                if (route.RouteOptimizationAlgorithm != model.RouteOptimizationAlgorithm)
                {
                    route.RouteOptimizationAlgorithm = model.RouteOptimizationAlgorithm;
                    isNeededToGenerateAgain = true;
                }
                if (route.RouteOptimizationProvider != model.RouteOptimizationProvider)
                {
                    route.RouteOptimizationProvider = model.RouteOptimizationProvider;
                    isNeededToGenerateAgain = true;
                }

                if (route.RouteOptimizationType != model.RouteOptimizationType)
                {
                    route.RouteOptimizationType = model.RouteOptimizationType;
                    isNeededToGenerateAgain = true;
                }



                if (!Equals(route.Shipments, shipments))
                {
                    route.Shipments = shipments;
                    isNeededToGenerateAgain = true;
                }

                if (isNeededToGenerateAgain)
                {
                    var generator = new RouteGenerator(route);

                    try
                    {
                        route = generator.GenerateRoute();
                    }
                    catch (Exception exception)
                    {
                        ModelState.AddModelError(string.Empty, "Wystąpił błąd!\n" + exception.Message + ((exception.InnerException != null) ? "\n" + exception.InnerException.Message : ""));
                        model.init();
                        return View(model);
                    }      
                }

                db.Entry(route).State = EntityState.Modified;


                try
                {
                    db.SaveChanges();
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "Nie udało się zapisać wygenerowanej trasy do bazy!\nPowód: " + exception.Message + ((exception.InnerException != null) ? "\n" + exception.InnerException.Message : ""));
                    model.init();
                    return View(model);
                }
                return RedirectToAction("Route", "Routes", new { id = route.Id });
                //return RedirectToAction("Calendar","Couriers",new{id=route.Driver});
            }


            model.init();
            return View(model);
        }


        [Authorize(Roles = "Pracownik")]
        // GET: Routes12/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Route route = db.Routes.Find(id);
            if (route == null)
            {
                return HttpNotFound();
            }
            return View(route);
        }

        [Authorize(Roles = "Pracownik")]
        // POST: Routes12/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Route route = db.Routes.Find(id);

            var driverId = route.Driver.Id;

            var notes = route.Notes;
            route.Notes.Clear();

            db.Notes.RemoveRange(notes);

            
            var segments = route.Segments;

            
            route.Segments.ForEach(s =>
            {
                var steps = s.Steps;
                s.Steps.Clear();
                //db.Steps.RemoveRange(steps);
            });
            
            
            route.Segments.Clear();
            //db.Segments.RemoveRange(segments);

            var shipments = route.Shipments;
            route.Shipments.Clear();

            //db.Shipments.RemoveRange(shipments);

            db.Routes.Remove(route);
            db.SaveChanges();

            return RedirectToAction("Calendar", "Couriers", new { id = driverId });
        }


        /*
        [HttpGet]
        [Authorize(Roles = "Pracownik")]
        public ActionResult EditRoute(string id)
        {
            var db = ApplicationDbContext.Create();

            var route = db.Routes.Find(id);


            var model = new EditRouteViewModel(route.Worker.Id)
            {
                Id = route.Id,
                Description = route.Description,
                DistanceMatrixProvider = route.DistanceMatrixProvider,
                DriverName = route.Driver.UserName,
                Notes = new List<Note>(route.Notes),
                Shipments = new List<Shipment>(route.Shipments),
                StartAddress = route.StartAddress,
                WorkerId = route.Worker.Id,
                StartDate = route.StartDateTime,
                RouteOptimizationAlgorithm = route.RouteOptimizationAlgorithm,
                RouteOptimizationProvider = route.RouteOptimizationProvider,
                RouteOptimizationType = route.RouteOptimizationType,
            };

            return View("EditRoute", model);
        }



        [HttpPost]
        [Authorize(Roles = "Admin, Pracownik")]
        public ActionResult EditRoute(EditRouteViewModel model)
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

                var IsNeededToGenerateAgain = false;

                var route = db.Routes.Find(model.Id);



                foreach (var shipment in route.Shipments)
                {
                    if(!model.Shipments.Contains(shipment,))
                }

                if(model.Shipments.Contains(route.))



                route = new Route
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
        */

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
                    ModelState.AddModelError(string.Empty, "Wystąpił błąd!\n" + exception.Message + ((exception.InnerException != null) ? "\n" + exception.InnerException.Message : ""));
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
                    ModelState.AddModelError(string.Empty, "Nie udało się zapisać wygenerowanej trasy do bazy!\nPowód: " + exception.Message + ((exception.InnerException != null) ? "\n" + exception.InnerException.Message : ""));
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
            //ApplicationDbContext db = ApplicationDbContext.Create();
            Route route = db.Routes.Find(id);

            return View("Route", route);
        }

        [Authorize(Roles = "Pracownik")]
        public ActionResult ShowDailyRoute(string id, DateTime date)
        {
            //ApplicationDbContext db = ApplicationDbContext.Create();

            var routes = db.Routes.ToList();

            Route route = routes.Find(r => ( r.Driver.Id == id  && r.StartDateTime.Date==date.Date));

            return View("Route", route);
        }


        [Authorize(Roles = "Kierowca")]
        public ActionResult ShowMyDailyRoute(DateTime date)
        {
            //ApplicationDbContext db = ApplicationDbContext.Create();

            var store = new UserStore<User>(db);
            var userManager = new UserManager<User>(store);
            var user = userManager.FindByName(System.Web.HttpContext.Current.User.Identity.Name);

            var routes = db.Routes.ToList();

            Route route = routes.Find(r => (r.Driver.Id == user.Id && r.StartDateTime.Date == date.Date));

            return View("Route", route);
        }


        [Authorize(Roles = "Kierowca")]
        public ActionResult RoutePrintVersion(string id)
        {
            //ApplicationDbContext db = ApplicationDbContext.Create();
            var route = db.Routes.Find(id);
            route.State = RouteState.InProgress;


            return View("RoutePrintVersion", route);
        }



        public ActionResult StartRoute(string id)
        {
            //ApplicationDbContext db = ApplicationDbContext.Create();
            var route = db.Routes.Find(id);
            route.State = RouteState.InProgress;
             
             db.Entry(route).State = EntityState.Modified;

            db.SaveChanges();
           // return RedirectToAction("ShowMyDailyRoute", new {date = route.StartDateTime});

            return View("RoutePrintVersion", route);
        }

        public ActionResult EndRoute(string id)
        {
            //ApplicationDbContext db = ApplicationDbContext.Create();
            var route = db.Routes.Find(id);
            route.State = RouteState.Completed;

            db.Entry(route).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("Index","Home");
        }

        public ActionResult SaveAsNewRoute(EditRouteViewModel editRouteViewModel)
        {
            return RedirectToAction("GenerateRoute", editRouteViewModel as GenerateRouteViewModel);
        }



        public bool Equals(ICollection<Shipment> shipments1, ICollection<Shipment> shipments2)
        {
            if (shipments1.Count != shipments2.Count)
                return false;
            else
            {
                var comparer = new ShipmentEqualityComparer();

                foreach (var shipment1 in shipments1)
                {
                    if (!shipments2.Contains(shipment1, comparer))
                        return false;
                }

                foreach (var shipment2 in shipments2)
                {
                    if (!shipments1.Contains(shipment2, comparer))
                        return false;
                }

                return true;
            }
        }
    }

    public class ShipmentEqualityComparer : IEqualityComparer<Shipment>
    {

        public bool Equals(Shipment x, Shipment y)
        {
            return x.DestinationAddress == y.DestinationAddress && x.Number == y.Number;
        }

        public int GetHashCode(Shipment obj)
        {
            return obj.DestinationAddress.GetHashCode() ^ obj.Number.GetHashCode();
        }
    }

}
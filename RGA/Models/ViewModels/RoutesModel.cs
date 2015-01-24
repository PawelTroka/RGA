using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Helpers;
using RGA.Helpers.RouteGeneration;

namespace RGA.Models.ViewModels
{
    public class RoutesViewModel
    {
        private readonly ApplicationDbContext routeDbContext = ApplicationDbContext.Create();

        public User Worker { get; set; }
        public Route Route { get; set; }

        public IEnumerable<Route> AllRoutes
        {
            get { return routeDbContext.Routes; }
        }

        public User Driver { get; set; }
        public DateTime Date { get; set; }
    }

    public class EditRouteViewModel : GenerateRouteViewModel
    {
        public EditRouteViewModel()
        {
            Shipments = new List<Shipment>
            {
                new Shipment {DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString()},
                new Shipment {DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString()}
            };
        }

        public EditRouteViewModel(Route route)
        {
            Id = route.Id;
            WorkerId = route.Worker.Id;
            Description = route.Description;
            DistanceMatrixProvider = route.DistanceMatrixProvider;
            DriverName = route.Driver.UserName;
            Note = (route.Notes != null && route.Notes.Count > 0) ? route.Notes.First().Content : null;
            Shipments = new List<Shipment>(route.Shipments);
            StartAddress = route.StartAddress;
            WorkerId = route.Worker.Id;
            StartDate = route.StartDateTime;
            RouteOptimizationAlgorithm = route.RouteOptimizationAlgorithm;
            RouteOptimizationProvider = route.RouteOptimizationProvider;
            RouteOptimizationType = route.RouteOptimizationType;
            State = route.State;
            AllowTollRoads = route.AllowTollRoads;
            init();
        }


        [HiddenInput(DisplayValue = false)]
        [Required]
        public string Id { get; set; }

        [Display(Name = "Adres końcowy")]
        public string EndAddress { get; set; }

        [Display(Name = "Stan")]
        public RouteState State { get; set; }
    }


    public class GenerateRouteViewModel
    {
        public GenerateRouteViewModel()
        {
            Shipments = new List<Shipment>
            {
                new Shipment {DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString()},
                new Shipment {DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString()}
            };
        }

        public GenerateRouteViewModel(string workerId)
        {
            WorkerId = workerId;
            init();
            Shipments = new List<Shipment>
            {
                new Shipment {DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString()},
                new Shipment {DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString()}
            };
        }

        [HiddenInput(DisplayValue = false)]
        [Required]
        public string WorkerId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Kierowca")]
        public string DriverName { get; set; }

        public SelectList MyDriversList { get; set; }

        [Required]
        [Display(Name = "Adres bazy")]
        public string StartAddress { get; set; }

        // [Required]
        [Display(Name = "Krótki opis trasy")]
        [DataType(DataType.MultilineText)]
        //public List<string> Addresses { get; set; }
        public string Description { get; set; }


        [Required]
        [Display(Name = "Przesyłki")]
        // [DataType(DataType.Custom)]
        public IList<Shipment> Shipments { get; set; }


        [Display(Name = "Notatki do trasy")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [Required]
        [Display(Name = "Dostawca macierzy odległości")]
        public DistanceMatrixProvider DistanceMatrixProvider { get; set; }

        [Required]
        [Display(Name = "Algorytm optymalizacji")]
        public RouteOptimizationAlgorithm RouteOptimizationAlgorithm { get; set; }

        [Required]
        [Display(Name = "Dostawca optymalizacji")]
        public RouteOptimizationProvider RouteOptimizationProvider { get; set; }

        [Required]
        [Display(Name = "Kryterium optymalizacji")]
        public RouteOptimizationType RouteOptimizationType { get; set; }


        [Display(Name = "Zezwalaj na drogi płatne?")]
        public bool AllowTollRoads { get; set; }

        public void init()
        {
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);
            User user = userManager.FindById(WorkerId);

            var driversList = new List<SelectListItem>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (User driver in user.Drivers)
            {
                driversList.Add(new SelectListItem {Value = driver.UserName, Text = driver.UserName});
            }

            MyDriversList = new SelectList(driversList, "Value", "Text");
        }
    }
}
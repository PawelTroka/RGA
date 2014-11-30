﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Helpers;

namespace RGA.Models.ViewModels
{
    public class RoutesViewModel
    {
        private ApplicationDbContext routeDbContext = ApplicationDbContext.Create();

        public User Worker { get; set; }
        public Route Route { get; set; }

        public IEnumerable<Route> AllRoutes { get { return routeDbContext.Routes; } }

        public User Driver { get; set; }
        public DateTime Date { get; set; }
    }


    public class GenerateRouteViewModel
    {
        private void init()
        {
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);
            var user = userManager.FindById(WorkerId);

            var driversList = new List<SelectListItem>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var driver in user.Drivers)
            {
                driversList.Add(new SelectListItem() { Value = driver.UserName, Text = driver.UserName });
            }

            MyDriversList = new SelectList(driversList, "Value", "Text");
        }

        public GenerateRouteViewModel()
        {
            Shipments = new List<Shipment>() { new Shipment() { DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString() }, new Shipment() { DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString() } };
        }

        public GenerateRouteViewModel(string workerId)
        {
            WorkerId = workerId;
            init();
            Shipments = new List<Shipment>() { new Shipment() { DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString() }, new Shipment() { DestinationAddress = "", Number = "", Id = Guid.NewGuid().ToString() } };
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
        public IEnumerable<Shipment> Shipments { get; set; }


        [Display(Name = "Notatki do trasy")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [Required]
        [Display(Name = "Algorytm optymalizacji trasy")]
        public RouteOptimizationAlgorithm RouteOptimizationAlgorithm { get; set; }

        [Required]
        [Display(Name = "Dostawca optymalizacji trasy")]
        public RouteOptimizationProvider RouteOptimizationProvider { get; set; }

        [Required]
        [Display(Name = "Kryterium optymalizacji trasy")]
        public RouteOptimizationType RouteOptimizationType { get; set; }
    }
}

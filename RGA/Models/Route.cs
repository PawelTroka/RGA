using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using GoogleMapsApi.Entities.Directions.Response;
using RGA.Helpers;

namespace RGA.Models
{
    public class Route
    {
        [Key]
        public string Id { get; set; }
        public byte[] Image { get; set; } //jpeg

        public string Description { get; set; }
        
        public string Summary { get; set; }
        public virtual ICollection<Note> Notes { get; set; }

      //  public virtual List<Leg> Sections { get; set; }// public virtual ICollection

        public DateTime StartDateTime { get; set; }

        public virtual User Driver { get; set; } //driver who drives that route

        public virtual User Worker { get; set; } //worker who created that route

        public string StartAddress { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; }
        public string EndAddress { get; set; }

        public TimeSpan Duaration { get; set; }
        public long Distance { get; set; }

        public RouteState State { get; set; }

        public RouteOptimizationAlgorithm RouteOptimizationAlgorithm { get; set; }

        public RouteOptimizationProvider RouteOptimizationProvider { get; set; }

        public RouteOptimizationType RouteOptimizationType { get; set; }
    }
}
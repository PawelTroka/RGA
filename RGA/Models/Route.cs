using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using GoogleMapsApi.Entities.Directions.Response;

namespace RGA.Models
{
    public class Route
    {


        public byte[] Image { get; set; } //jpeg
        public string Summary { get; set; }
        public List<string> Notes { get; set; }

        public List<Leg> Sections { get; set; }

        public DateTime StartDateTime { get; set; }

        public ApplicationUser Driver { get; set; } //driver who drives that route

        public ApplicationUser Worker { get; set; } //worker who created that route

        public string StartAddress { get; set; }
        public List<string> Addresses { get; set; }

        public TimeSpan Duaration { get; set; }
        public long Distance { get; set; }
    }



    public class RouteDBContext : DbContext
    {
        public RouteDBContext()
            : base("DefaultConnection")
        {
            
        }

        public static RouteDBContext Create()
        {
            return new RouteDBContext();
        }

        public DbSet<Route> Routes { get; set; }
    }
}
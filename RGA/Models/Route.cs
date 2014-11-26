using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace RGA.Models
{
    public class Route
    {



        public List<string> Notes { get; set; }

        public DateTime StartDateTime { get; set; }

        public ApplicationUser Driver { get; set; }

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
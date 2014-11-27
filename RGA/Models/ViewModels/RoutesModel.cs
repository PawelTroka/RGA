using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RGA.Models.ViewModels
{
    public class RoutesViewModel
    {
        private RouteDBContext routeDbContext = RouteDBContext.Create();

        public ApplicationUser Worker { get; set; }
        public Route Route { get; set; }

        public IEnumerable<Route> AllRoutes { get { return routeDbContext.Routes; } } 

        public ApplicationUser Driver { get; set; }
        public DateTime Date { get; set; }
    }
}

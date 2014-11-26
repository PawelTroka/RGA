using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Google.Maps;
using Google.Maps.Elevation;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using RGA.Models;
using TravelMode = Google.Maps.TravelMode;

namespace RGA.Helpers
{
    public class RouteGenerator
    {
        private TspSolver tspSolver;

        //private GoogleMapsApi.Entities.Common. googleMaps;
        private string browserAPIKey = "AIzaSyCBup_64x3U1wfm3NTiu1mO_8tyz1GAM6Q";

        private string serverAPIKey = "AIzaSyDPhoaf5psgGK27m6FWKTyLiTf-aIiHiWs";
        public RouteGenerator()
        {
            Addresses =  new List<string>();
         //   googleMaps = new GoogleMaps();
            
        }

        public RouteOptimizationProvider routeOptimizationProvider { get; set; }
        public RouteOptimizationAlgorithm routeOptimizationAlgorithm { get; set; }
        public RouteOptimizationType routeOptimizationType { get; set; }
        public string BaseAddress { get; set; }
        public List<string> Addresses { get; set; }


        
        public RGA.Models.Route GenerateRoute()
        {


            if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
            {
               // Google.Maps.Direction.DirectionRequest 

               var  directionsRequest = new DirectionsRequest()
                {
                    Origin = BaseAddress,
                    Destination = BaseAddress,
                    Waypoints = Addresses.ToArray(),
                    Language = "pl",
                    Avoid = AvoidWay.Tolls,
                    OptimizeWaypoints = true //solves TSP for us, but is limited to something like 8 points
                };

            
                DirectionsResponse directions = GoogleMaps.Directions.Query(directionsRequest);

                

                if(directions.Status != DirectionsStatusCodes.OK)
                    throw new Exception("Nie udało się wygenerować trasy!\nPowód: "+directions.StatusStr);


            }
            else if (routeOptimizationProvider == RouteOptimizationProvider.RGA)
            {
                var waypoints = new SortedList<int, Google.Maps.Waypoint> {{0, new Waypoint() {Address = BaseAddress}}};

                for (int i = 0; i < Addresses.Count; i++)
                {
                    waypoints.Add(i+1,new Waypoint(){Address = Addresses[i]});
                }

                var request = new Google.Maps.DistanceMatrix.DistanceMatrixRequest()
                {
                    Language = "pl",
                    Avoid = Avoid.tolls | Avoid.ferries,
                    WaypointsOrigin = waypoints,
                    WaypointsDestination = waypoints,
                    Units = Units.metric,
                    Mode = TravelMode.driving
                };


                var response = new Google.Maps.DistanceMatrix.DistanceMatrixService().GetResponse(request);

                if (response.Status != ServiceResponseStatus.Ok)
                    throw new Exception("Nie udało się wygenerować trasy!\nPowód: " + response.Status.ToString());


                var costs = new double[Addresses.Count + 1, Addresses.Count + 1];
                var indices = new int[Addresses.Count + 1];

                for (int i = 0; i < response.Rows.Length; i++)
                {
                    indices[i] = i;
                    for (int j = 0; j < response.Rows[i].Elements.Length; j++)
                    {
                        if (routeOptimizationType == RouteOptimizationType.Time)
                            costs[i, j] = long.Parse(response.Rows[i].Elements[j].duration.Value);
                        
                        else if (routeOptimizationType == RouteOptimizationType.Distance)
                            costs[i, j] = long.Parse(response.Rows[i].Elements[j].distance.Value);
                    }
                    
                }


                tspSolver = new HeldKarpTSPSolver(indices,costs);

                double cost;

                var optimalRoute = tspSolver.Solve(out cost);

                var route = new RGA.Models.Route();
                
                route.StartAddress = BaseAddress;

                foreach (var item in optimalRoute)
                {
                    if (item != 0)
                        route.Addresses.Add(Addresses[item - 1]);
                    if (item < Addresses.Count + 1)
                    {
                        route.Duaration +=
                            TimeSpan.FromSeconds(long.Parse(response.Rows[item].Elements[item + 1].duration.Value));

                        route.Distance += long.Parse(response.Rows[item].Elements[item + 1].distance.Value);
                    }
                }

                return route;

            }


            return new RGA.Models.Route();
        }
    }
}
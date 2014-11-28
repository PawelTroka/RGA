using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using Google.Maps.Elevation;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using RGA.Models;
using WebGrease.Css.Extensions;
using Route = RGA.Models.Route;
using TravelMode = Google.Maps.TravelMode;

namespace RGA.Helpers
{
    public class RouteGenerator
    {
        private Route route;
        private TspSolver tspSolver;
        private string browserAPIKey = "AIzaSyCBup_64x3U1wfm3NTiu1mO_8tyz1GAM6Q";
        private string serverAPIKey = "AIzaSyDPhoaf5psgGK27m6FWKTyLiTf-aIiHiWs";
        public RouteGenerator()
        {
            Addresses = new List<string>();
            route = new RGA.Models.Route();
        }

        public RouteOptimizationProvider routeOptimizationProvider { get; set; }
        public RouteOptimizationAlgorithm routeOptimizationAlgorithm { get; set; }
        public RouteOptimizationType routeOptimizationType { get; set; }
        public string BaseAddress { get; set; }
        public List<string> Addresses { get; set; }

        public RGA.Models.Route GenerateRoute()
        {
            if (routeOptimizationProvider == RouteOptimizationProvider.RGA)
                SortThingsAccordingToCost();

            var directions = getDirections();

            route.Summary = directions.Routes.First().Summary;
         //   route.Sections = new List<Leg>(directions.Routes.First().Legs);

            if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                SortThingsAccordingToWaypointOrder(directions.Routes.First().WaypointOrder);

            directions.Routes.First().Legs.ForEach(leg => route.Duaration += leg.Duration.Value);
            directions.Routes.First().Legs.ForEach(leg => route.Distance += leg.Distance.Value);

            route.Addresses = Addresses;
            route.StartAddress = BaseAddress;
            route.Image = getImageBytes();

            return route;
        }

        private void SortThingsAccordingToCost()
        {
            var response = getDistanceMatrix();

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

            tspSolver = new HeldKarpTSPSolver(indices, costs);

            double cost;

            var optimalRoute = tspSolver.Solve(out cost);

            var tmpAddress = new List<string>();

            foreach (var item in optimalRoute)
            {
                if (item != 0)
                    tmpAddress.Add(Addresses[item - 1]);
            }
            Addresses = tmpAddress;
        }

        private void SortThingsAccordingToWaypointOrder(int[] order)
        {
            Addresses.Sort((a1, a2) => order[Addresses.IndexOf(a1)].CompareTo(order[Addresses.IndexOf(a2)]));

            var orderOfSections = new List<int> { 0 };
            orderOfSections.AddRange(order);
            for (int i = 1; i < orderOfSections.Count; i++)
                orderOfSections[i]++;
            orderOfSections.Add(0);
         //   route.Sections.Sort(
             //   (s1, s2) => orderOfSections[route.Sections.IndexOf(s1)].CompareTo(orderOfSections[route.Sections.IndexOf(s2)]));
        }


        private DistanceMatrixResponse getDistanceMatrix()
        {
            var waypoints = new SortedList<int, Google.Maps.Waypoint> { { 0, new Waypoint() { Address = BaseAddress } } };

            for (int i = 0; i < Addresses.Count; i++)
            {
                waypoints.Add(i + 1, new Waypoint() { Address = Addresses[i] });
            }

            var request = new Google.Maps.DistanceMatrix.DistanceMatrixRequest()
            {
                Language = "pl",
                Avoid = Avoid.tolls, //| Avoid.ferries,
                WaypointsOrigin = waypoints,
                WaypointsDestination = waypoints,
                Units = Units.metric,
                Mode = TravelMode.driving
            };


            var response = new Google.Maps.DistanceMatrix.DistanceMatrixService().GetResponse(request);

            if (response.Status != ServiceResponseStatus.Ok)
                throw new Exception("Nie udało się wygenerować trasy!\nPowód: " + response.Status.ToString());

            return response;
        }

        private DirectionsResponse getDirections()
        {
            var directionsRequest = new DirectionsRequest()
            {
                Origin = BaseAddress,
                Destination = BaseAddress,
                Waypoints = Addresses.ToArray(),
                Language = "pl",
                Avoid = AvoidWay.Tolls,
            };


            if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                directionsRequest.OptimizeWaypoints = true; //solves TSP for us, but is limited to something like 8 points

            DirectionsResponse directions = GoogleMaps.Directions.Query(directionsRequest);

            if (directions.Status != DirectionsStatusCodes.OK)
                throw new Exception("Nie udało się wygenerować trasy!\nPowód: " + directions.StatusStr);

            return directions;
        }


        private byte[] getImageBytes()
        {
            var locations = new List<Location>() { BaseAddress };

            foreach (var address in Addresses)
            {
                locations.Add(address);
            }
            locations.Add(BaseAddress);

            var mapRequest = new Google.Maps.StaticMaps.StaticMapRequest()
            {
                Language = "pl",
                //MapType = Mapt
                Format = GMapsImageFormats.JPG,
                Path = new Path(locations),
                // Size = new Size(600,600)
            };
            //route.Sections.First().Steps.First().

            return (new Google.Maps.StaticMaps.StaticMapService()).GetImageBytes(mapRequest);
        }
    }
}
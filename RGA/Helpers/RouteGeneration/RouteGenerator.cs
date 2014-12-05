extern alias OSM;

using System;
using System.Collections.Generic;
using System.Linq;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using Google.Maps.StaticMaps;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using OsmSharp.Osm;
using OsmSharp.Osm.API;
using OsmSharp.Osm.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.TSP;
using RGA.Helpers.RouteGeneration;
using RGA.Helpers.TSP;
using RGA.Models;
using WebGrease.Css.Extensions;
using Route = RGA.Models.Route;
using Step = RGA.Models.Step;
using TravelMode = Google.Maps.TravelMode;


namespace RGA.Helpers
{
    public class RouteGenerator
    {
        private readonly Route route;
        private string browserAPIKey = "AIzaSyCBup_64x3U1wfm3NTiu1mO_8tyz1GAM6Q";
        private string serverAPIKey = "AIzaSyDPhoaf5psgGK27m6FWKTyLiTf-aIiHiWs";
        private TspSolver tspSolver;

        private string mapQuestAPIKey = "Fmjtd%7Cluurn16anq%2C85%3Do5-9wtsga";

        public RouteGenerator(Route route)
        {
            Addresses = new List<string>();
            this.route = route;
            BaseAddress = route.StartAddress;
            route.Shipments.ForEach(s => Addresses.Add(s.DestinationAddress));
            routeOptimizationAlgorithm = route.RouteOptimizationAlgorithm;
            routeOptimizationProvider = route.RouteOptimizationProvider;
            routeOptimizationType = route.RouteOptimizationType;
            distanceMatrixProvider = route.DistanceMatrixProvider;
        }


        public DistanceMatrixProvider distanceMatrixProvider { get; set; }
        public RouteOptimizationProvider routeOptimizationProvider { get; set; }
        public RouteOptimizationAlgorithm routeOptimizationAlgorithm { get; set; }
        public RouteOptimizationType routeOptimizationType { get; set; }
        public string BaseAddress { get; set; }
        public List<string> Addresses { get; set; }

        public Route GenerateRoute()
        {
            if (routeOptimizationProvider == RouteOptimizationProvider.RGA)
                SortThingsAccordingToCost();
            else if (routeOptimizationProvider == RouteOptimizationProvider.MapQuest)
            {
                var mapQuest = new MapQuestAPI();
                var listOfAllAddresses = new List<string>() { BaseAddress };
                listOfAllAddresses.AddRange(Addresses);
                listOfAllAddresses.Add(BaseAddress);
                var optimalRoute = mapQuest.getOptimalRoute(listOfAllAddresses);
                var waypointOrder = new List<int>();

                for (int i = 1; i < optimalRoute.Length - 1; i++)
                    waypointOrder.Add(optimalRoute[i]);
                SortThingsAccordingToWaypointOrder(waypointOrder.ToArray());
            }


            //DirectionsResponse directions = getDirections();
            var responseRoute = getRoute();

            route.Summary = responseRoute.Summary;
            //   route.Sections = new List<Leg>(directions.Routes.First().Legs);
            // directions.Routes.First().Legs.First().Steps.First().;
            if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                SortThingsAccordingToWaypointOrder(responseRoute.WaypointOrder);


            responseRoute.Legs.ForEach(leg => route.Duaration += leg.Duration.Value);
            responseRoute.Legs.ForEach(leg => route.Distance += leg.Distance.Value);

            //route.Shipments// = Addresses;

            var shipments = new List<Shipment>(route.Shipments);
            shipments.Sort(
                (s1, s2) => Addresses.IndexOf(s1.DestinationAddress).CompareTo(Addresses.IndexOf(s2.DestinationAddress)));
            route.Shipments = shipments;

            route.StartAddress = BaseAddress;

            //   var locationsPoints = new List<Location>();

            // responseRoute
            //   .OverviewPath.Points.ForEach(p => locationsPoints.Add(new Location(p.LocationString)));


            var segments = new List<Segment>();
            responseRoute.Legs.ForEach(
                l =>
                {
                    segments.Add(new Segment()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Distance = l.Distance.Value,
                        Duaration = l.Duration.Value,
                        StartAddress = l.StartAddress,
                        EndAddress = l.EndAddress,
                        Steps = new List<Step>()
                    });

                    l.Steps.ForEach(s => segments.Last().Steps.Add(new Step()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Distance = s.Distance.Value,
                        Duaration = s.Duration.Value,
                        EndLocation = s.EndLocation.LocationString,
                        HtmlInstructions = s.HtmlInstructions,
                        StartLocation = s.StartLocation.LocationString
                    }));
                }
                );

            route.Segments = segments;
            route.Image = getImageBytes();

            return route;
        }

        private void SortThingsAccordingToCost()
        {
            var costs = new double[Addresses.Count + 1, Addresses.Count + 1];
            var indices = Enumerable.Range(0, Addresses.Count + 1).ToArray();


            switch (distanceMatrixProvider)
            {
                case DistanceMatrixProvider.GoogleMaps:
                    var response = getDistanceMatrix();

                    for (int i = 0; i < response.Rows.Length; i++)
                        for (int j = 0; j < response.Rows[i].Elements.Length; j++)
                        {
                            if (routeOptimizationType == RouteOptimizationType.Time)
                                costs[i, j] = long.Parse(response.Rows[i].Elements[j].duration.Value);

                            else if (routeOptimizationType == RouteOptimizationType.Distance)
                                costs[i, j] = long.Parse(response.Rows[i].Elements[j].distance.Value);
                        }
                    break;


                case DistanceMatrixProvider.MapQuest:
                    var mapQuest = new MapQuestAPI();
                    var listOfAllAddresses = new List<string>() { BaseAddress };
                    listOfAllAddresses.AddRange(Addresses);
                    //listOfAllAddresses.Add(BaseAddress);
                    costs = mapQuest.getDistanceMatrix(listOfAllAddresses, routeOptimizationType);
                    break;

                case DistanceMatrixProvider.BingMaps:
                    throw new NotImplementedException("DistanceMatrixProvider.BingMaps" + " nie jest jeszcze zaimplementowany");
                    break;

                case DistanceMatrixProvider.OpenStreetMap:
                    throw new NotImplementedException("DistanceMatrixProvider.OpenStreetMap" + " nie jest jeszcze zaimplementowany");
                    break;

                default:
                    throw new NotImplementedException(distanceMatrixProvider.ToString() + " nie jest jeszcze zaimplementowany");
                    break;
            }


            switch (routeOptimizationAlgorithm)
            {
                case RouteOptimizationAlgorithm.BruteForce:
                    tspSolver = new BruteForceTSPSolver(indices, costs);
                    break;
                case RouteOptimizationAlgorithm.HeldKarp:
                    tspSolver = new HeldKarpTSPSolver(indices, costs);
                    break;

                default:
                    tspSolver = new HeldKarpTSPSolver(indices, costs);
                    break;
            }

            double cost;

            IEnumerable<int> optimalRoute = tspSolver.Solve(out cost);

            var tmpAddress = (from item in optimalRoute where item != 0 select Addresses[item - 1]).ToList();

            Addresses = tmpAddress;
        }

        private void SortThingsAccordingToWaypointOrder(int[] order)
        {
            var copyOfAddresses = new List<string>(Addresses);
            Addresses.Sort((a1, a2) => order[copyOfAddresses.IndexOf(a1)].CompareTo(order[copyOfAddresses.IndexOf(a2)]));

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
            var waypoints = new SortedList<int, Waypoint> { { 0, new Waypoint { Address = BaseAddress } } };

            for (int i = 0; i < Addresses.Count; i++)
            {
                waypoints.Add(i + 1, new Waypoint { Address = Addresses[i] });
            }

            /*
             * Users of the free API:
            100 elements per query.
            100 elements per 10 seconds.
            2 500 elements per 24 hour period.       
             */
            var request = new DistanceMatrixRequest
            {
                Language = "pl",
                Avoid = Avoid.tolls, //| Avoid.ferries,
                WaypointsOrigin = waypoints,
                WaypointsDestination = waypoints,
                Units = Units.metric,
                Mode = TravelMode.driving,
                Sensor = false
            };


            DistanceMatrixResponse response = new DistanceMatrixService().GetResponse(request);

            if (response.Status != ServiceResponseStatus.Ok)
                throw new Exception("Nie udało się wygenerować trasy!\nPowód: " + response.Status);

            return response;
        }


        private GoogleMapsApi.Entities.Directions.Response.Route getRoute()
        {

            GoogleMapsApi.Entities.Directions.Response.Route responseRoute = null;
            DirectionsRequest directionsRequest = null;
            DirectionsResponse directions = null;

            var summaries = "";
            var legs = new List<Leg>();
            var waypointsOrder = new List<int>();
            var warnings = new List<string>();


            var allAddresses = new List<string>() { BaseAddress };
            allAddresses.AddRange(Addresses);
            allAddresses.Add(BaseAddress);


            for (int i = 0; i < allAddresses.Count - 1; i += 9) //because free version of GoogleMaps Directions API is limited to maximum 8 waypoints per request
            {

                directionsRequest = new DirectionsRequest
                {
                    Origin = allAddresses[i],
                    Destination = (i + 9 < allAddresses.Count) ? allAddresses[i + 9] : allAddresses.Last(),
                    Waypoints = allAddresses.GetRange(i + 1, Math.Min(8, allAddresses.Count - i - 2)).ToArray(),
                    Language = "pl",
                    Avoid = AvoidWay.Tolls,
                };

                if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                    directionsRequest.OptimizeWaypoints = true;//solves TSP for us, but is limited to 8 points

                directions = GoogleMaps.Directions.Query(directionsRequest);

                if (directions.Status != DirectionsStatusCodes.OK)
                    throw new Exception("Nie udało się wygenerować trasy!\nPowód: " + directions.StatusStr);
                else
                {
                    legs.AddRange(directions.Routes.First().Legs);
                    summaries += directions.Routes.First().Summary + Environment.NewLine;
                    if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                        waypointsOrder.AddRange(directions.Routes.First().WaypointOrder);
                    warnings.AddRange(directions.Routes.First().Warnings);
                }
            }


            responseRoute = new GoogleMapsApi.Entities.Directions.Response.Route()
            {
                Legs = legs,
                Summary = summaries,
                Warnings = warnings.ToArray()
            };

            if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                responseRoute.WaypointOrder = waypointsOrder.ToArray();


            return responseRoute;
        }


        private DirectionsResponse getDirections()
        {
            var directionsRequest = new DirectionsRequest
            {
                Origin = BaseAddress,
                Destination = BaseAddress,
                Waypoints = Addresses.ToArray(),
                Language = "pl",
                Avoid = AvoidWay.Tolls,
            };


            if (routeOptimizationProvider == RouteOptimizationProvider.GoogleMaps)
                directionsRequest.OptimizeWaypoints = true;
            //solves TSP for us, but is limited to something like 8 points

            DirectionsResponse directions = GoogleMaps.Directions.Query(directionsRequest);


            if (directions.Status != DirectionsStatusCodes.OK)
                throw new Exception("Nie udało się wygenerować trasy!\nPowód: " + directions.StatusStr);

            return directions;
        }


        private byte[] getImageBytes()
        {
            var locations = new List<Location> { BaseAddress };
            locations.AddRange(Addresses.Select(address => (Location)address));

            locations.Add(BaseAddress);

            var markers = new MapMarkersCollection();

            locations.ForEach(markers.Add);

            var mapRequest = new StaticMapRequest
            {
                Language = "pl",
                //MapType = Mapt
                Format = GMapsImageFormats.JPG,
                Markers = markers,
                Path = new Path(locations),
                Sensor = false
                // Size = new Size(600,600)
            };
            //route.Sections.First().Steps.First().

            return (new StaticMapService()).GetImageBytes(mapRequest);
        }
    }
}
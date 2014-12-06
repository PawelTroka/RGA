using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using RGA.Helpers;
using RGA.Helpers.RouteGeneration;

namespace RGA.Models
{
    public class Route
    {
        [Key]
        public string Id { get; set; }

        [Display(Name = "Obraz")]
        public byte[] Image { get; set; } //jpeg

        [Display(Name = "Opis")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Podsumowanie")]
        public string Summary { get; set; }

        [Display(Name = "Notatki")]
        public virtual ICollection<Note> Notes { get; set; }

        //  public virtual List<Leg> Sections { get; set; }// public virtual ICollection

        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "Kierowca")]
        public virtual User Driver { get; set; } //driver who drives that route

        [Display(Name = "Pracownik")]
        public virtual User Worker { get; set; } //worker who created that route

        [Display(Name = "Adres początkowy")]
        public string StartAddress { get; set; }

        [Display(Name = "Przesyłki")]
        public virtual ICollection<Shipment> Shipments { get; set; }

        [Display(Name = "Segmenty trasy")]
        public virtual ICollection<Segment> Segments { get; set; }

        [Display(Name = "Adres końcowy")]
        public string EndAddress { get; set; }

        [Display(Name = "Czas trwania")]
        [DataType(DataType.Time)]
        [NotMapped]
        public TimeSpan Duaration
        {
            get { return TimeSpan.FromTicks(DuarationTicks); }
            set { DuarationTicks = value.Ticks; }
        }

        public long DuarationTicks { get; set; }


        [Display(Name = "Dystans")]
        public long Distance { get; set; }


        [Display(Name = "Stan")]
        public RouteState State { get; set; }

        [Display(Name = "Dostawca macierzy odległości")]
        public DistanceMatrixProvider DistanceMatrixProvider { get; set; }

        [Display(Name = "Algorytm optymalizacji")]
        public RouteOptimizationAlgorithm RouteOptimizationAlgorithm { get; set; }

        [Display(Name = "Dostawca optymalizacji")]
        public RouteOptimizationProvider RouteOptimizationProvider { get; set; }

        [Display(Name = "Kryterium optymalizacji")]
        public RouteOptimizationType RouteOptimizationType { get; set; }


        [Display(Name = "Wizualizacja trasy w systemie map online")]
        [DataType(DataType.Html)]
        public string DynamicMapHtml
        {
            get
            {
                if (Shipments == null)
                    return "";
                if (Shipments.Count > 8)
                {
                    var template = new StringBuilder(@"<style>
    #map-canvas425321423y78ydgf23dqd1d21f12t87 {
        height: 500px;
        width: 800px;
    }
</style>
<script src=""https://maps.googleapis.com/maps/api/js?sensor=false&language=pl""></script>
<script>
    function initializeMap425321423y78ydgf23dqd1d21f12t87() {
        var stops = [{ADDRESSES_COMMA_SEPARATED_STRINGS}];

        var map = new window.google.maps.Map(document.getElementById(""map-canvas425321423y78ydgf23dqd1d21f12t87""));

        // new up complex objects before passing them around
        var directionsDisplay = new window.google.maps.DirectionsRenderer();
        var directionsService = new window.google.maps.DirectionsService();

        Tour_startUp(stops);

        window.tour.loadMap(map, directionsDisplay);
        //window.tour.fitBounds(map);

        if (stops.length > 1)
            window.tour.calcRoute(directionsService, directionsDisplay);
    };

    function Tour_startUp(stops) {
        var gdansk = new google.maps.LatLng(54.371906, 18.616290);
        if (!window.tour) window.tour = {
            updateStops: function (newStops) {
                stops = newStops;
            },
            // map: google map object
            // directionsDisplay: google directionsDisplay object (comes in empty)
            loadMap: function (map, directionsDisplay) {
                var myOptions = {
                    zoom: 6,
                    center: gdansk, // default to Gdansk
                    mapTypeId: window.google.maps.MapTypeId.ROADMAP
                };
                map.setOptions(myOptions);
                directionsDisplay.setMap(map);
            },
            calcRoute: function (directionsService, directionsDisplay) {
                var batches = [];
                var itemsPerBatch = 10; // google API max = 10 - 1 start, 1 stop, and 8 waypoints
                var itemsCounter = 0;
                var wayptsExist = stops.length > 0;

                while (wayptsExist) {
                    var subBatch = [];
                    var subitemsCounter = 0;

                    for (var j = itemsCounter; j < stops.length; j++) {
                        subitemsCounter++;
                        subBatch.push({
                            location: stops[j],
                            stopover: true
                        });
                        if (subitemsCounter == itemsPerBatch)
                            break;
                    }

                    itemsCounter += subitemsCounter;
                    batches.push(subBatch);
                    wayptsExist = itemsCounter < stops.length;
                    // If it runs again there are still points. Minus 1 before continuing to
                    // start up with end of previous tour leg
                    itemsCounter--;
                }

                // now we should have a 2 dimensional array with a list of a list of waypoints
                var combinedResults;
                var unsortedResults = [{}]; // to hold the counter and the results themselves as they come back, to later sort
                var directionsResultsReturned = 0;

                for (var k = 0; k < batches.length; k++) {
                    var lastIndex = batches[k].length - 1;
                    var start = batches[k][0].location;
                    var end = batches[k][lastIndex].location;

                    // trim first and last entry from array
                    var waypts = [];
                    waypts = batches[k];
                    waypts.splice(0, 1);
                    waypts.splice(waypts.length - 1, 1);

                    var request = {
                        origin: start,
                        destination: end,
                        waypoints: waypts,
                        optimizeWaypoints: false,
                        avoidTolls: true,
                        travelMode: window.google.maps.TravelMode.DRIVING
                    };
                    (function (kk) {
                        directionsService.route(request, function (result, status) {
                            if (status == window.google.maps.DirectionsStatus.OK) {

                                var unsortedResult = { order: kk, result: result };
                                unsortedResults.push(unsortedResult);

                                directionsResultsReturned++;

                                if (directionsResultsReturned == batches.length) // we've received all the results. put to map
                                {
                                    // sort the returned values into their correct order
                                    unsortedResults.sort(function (a, b) { return parseFloat(a.order) - parseFloat(b.order); });
                                    var count = 0;
                                    for (var key in unsortedResults) {
                                        if (unsortedResults[key].result != null) {
                                            if (unsortedResults.hasOwnProperty(key)) {
                                                if (count == 0) // first results. new up the combinedResults object
                                                    combinedResults = unsortedResults[key].result;
                                                else {
                                                    // only building up legs, overview_path, and bounds in my consolidated object. This is not a complete
                                                    // directionResults object, but enough to draw a path on the map, which is all I need
                                                    combinedResults.routes[0].legs = combinedResults.routes[0].legs.concat(unsortedResults[key].result.routes[0].legs);
                                                    combinedResults.routes[0].overview_path = combinedResults.routes[0].overview_path.concat(unsortedResults[key].result.routes[0].overview_path);

                                                    combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(unsortedResults[key].result.routes[0].bounds.getNorthEast());
                                                    combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(unsortedResults[key].result.routes[0].bounds.getSouthWest());
                                                }
                                                count++;
                                            }
                                        }
                                    }
                                    directionsDisplay.setDirections(combinedResults);
                                }
                            }
                        });
                    })(k);
                }
            }
        };
    }

    google.maps.event.addDomListener(window, 'load', initializeMap425321423y78ydgf23dqd1d21f12t87);
</script>
<div id=""map-canvas425321423y78ydgf23dqd1d21f12t87""></div>");


                    var waypoints = new StringBuilder();

                    const string str = @"'{ADDRESS_HERE}',";

                    waypoints.Append(str.Replace("{ADDRESS_HERE}", this.StartAddress));

                    foreach (Shipment shipment in Shipments)
                    {
                        waypoints.Append(str.Replace("{ADDRESS_HERE}", shipment.DestinationAddress));
                    }


                    waypoints.Append(string.IsNullOrEmpty(EndAddress)
                        ? str.Replace("{ADDRESS_HERE}", this.StartAddress)
                        : str.Replace("{ADDRESS_HERE}", this.EndAddress));

                    waypoints.Remove(waypoints.Length - 1, 1); //remove last comma

                    template.Replace(@"{ADDRESSES_COMMA_SEPARATED_STRINGS}", waypoints.ToString());

                    return template.ToString();
                }
                else
                {
                    var template = new StringBuilder(@"<style>
    #map-canvas425321423y78ydgf23t87 {
        height: 500px;
        width: 800px;
    }
</style>
<script src=""https://maps.googleapis.com/maps/api/js?sensor=false&language=pl""></script>
<script>
    function initializeMap425321423y78ydgf23t87() {

        var directionsService = new google.maps.DirectionsService();
        var directionsDisplay = new google.maps.DirectionsRenderer();
        var gdansk = new google.maps.LatLng(54.371906, 18.616290);
        var mapOptions = {
            zoom: 6,
            center: gdansk
        };
        var map = new google.maps.Map(document.getElementById('map-canvas425321423y78ydgf23t87'), mapOptions);
        directionsDisplay.setMap(map);

        var start = '{START_ADDRESS_STRING}';
        var end = '{END_ADDRESS_STRING}';
        var waypts = [{ADDRESSES_COMMA_SEPARATED_WAYPOINTS_STRINGS}];

        var request = {
            origin: start,
            destination: end,
            waypoints: waypts,
            optimizeWaypoints: false,
            avoidTolls: true,
            travelMode: google.maps.TravelMode.DRIVING
        };

        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(response);
            }
        });
    }

    google.maps.event.addDomListener(window, 'load', initializeMap425321423y78ydgf23t87);
</script>
<div id=""map-canvas425321423y78ydgf23t87""></div>");

                    template.Replace(@"{START_ADDRESS_STRING}", StartAddress);

                    template.Replace(@"{END_ADDRESS_STRING}",
                        string.IsNullOrEmpty(EndAddress) ? StartAddress : EndAddress);

                    var waypoints = new StringBuilder();

                    const string str = @"{location: '{ADDRESS_HERE}',stopover:true},";
                    foreach (Shipment shipment in Shipments)
                    {
                        waypoints.Append(str.Replace("{ADDRESS_HERE}", shipment.DestinationAddress));
                    }
                    waypoints.Remove(waypoints.Length - 1, 1); //remove last comma

                    template.Replace(@"{ADDRESSES_COMMA_SEPARATED_WAYPOINTS_STRINGS}", waypoints.ToString());

                    return template.ToString();
                }
            }
        }
    }
}
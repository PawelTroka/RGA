using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
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
                var template = new StringBuilder(@"<style>
    #map-canvas425321423y78ydgf23t87 {
        width: 500px;
        height: 400px;
    }
</style>
<script src=""https://maps.googleapis.com/maps/api/js""></script>
<script>
function initializeMap425321423y78ydgf23t87() {

    var directionsService = new google.maps.DirectionsService();
    var  directionsDisplay = new google.maps.DirectionsRenderer();
    var gdansk = new google.maps.LatLng(54.371906, 18.616290);
    var mapOptions = {
        zoom: 6,
        center: gdansk
    }
    var map = new google.maps.Map(document.getElementById('map-canvas425321423y78ydgf23t87'), mapOptions);
    directionsDisplay.setMap(map);

    var start = '{START_ADDRESS_STRING}';
    var end = '{END_ADDRESS_STRING}';
    var waypts = [{ADDRESSES_COMMA_SEPARATED_WAYPOINTS_STRINGS}];//{location: 'Krakow, Poland',stopover:true}

    var request = {
        origin: start,
        destination: end,
        waypoints: waypts,
        optimizeWaypoints: false,
        travelMode: google.maps.TravelMode.DRIVING
    };

    directionsService.route(request, function(response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
        }
    });
}
google.maps.event.addDomListener(window, 'load', initializeMap425321423y78ydgf23t87);
</script>
<div id=""map-canvas425321423y78ydgf23t87""></div>");

                template.Replace(@"{START_ADDRESS_STRING}", this.StartAddress);

                template.Replace(@"{END_ADDRESS_STRING}",
                    string.IsNullOrEmpty(this.EndAddress) ? this.StartAddress : this.EndAddress);

                var waypoints = new StringBuilder();

                const string str = @"{location: '{ADDRESS_HERE}',stopover:true},";
                foreach (var shipment in Shipments)
                {
                    waypoints.Append(str.Replace("{ADDRESS_HERE}", shipment.DestinationAddress));
                }
                waypoints.Remove(waypoints.Length - 1, 1);//remove last comma

                template.Replace(@"{ADDRESSES_COMMA_SEPARATED_WAYPOINTS_STRINGS}", waypoints.ToString());

                return template.ToString();
            }
        }
    }
}
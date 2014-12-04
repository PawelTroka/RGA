using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Google.Maps;
using GoogleMapsApi.Entities.Directions.Response;

namespace RGA.Models
{
    // Summary:
    //     Each element in the legs array specifies a single leg of the journey from
    //     the origin to the destination in the calculated route. For routes that contain
    //     no waypoints, the route will consist of a single "leg," but for routes that
    //     define one or more waypoints, the route will consist of one or more legs,
    //     corresponding to the specific legs of the journey.
    public class Segment //base on Leg class from GoogleMapsAPI
    {
        [Key]
        public string Id { get; set; }
        //
        // Summary:
        //     distance indicates the total distance covered by this leg
        public long Distance { get; set; }
        //
        // Summary:
        //     duration indicates the total duration of this leg,
        [Display(Name = "Czas trwania")]
        [DataType(DataType.Time)]
        [NotMapped]
        public TimeSpan Duaration
        {
            get { return TimeSpan.FromTicks(DuarationTicks); }
            set { DuarationTicks = value.Ticks; }
        }

        public long DuarationTicks { get; set; }
        //
        // Summary:
        //     end_addresss contains the human-readable address (typically a street address)
        //     reflecting the end_location of this leg.
        public string EndAddress { get; set; }
        //
        // Summary:
        //     start_address contains the human-readable address (typically a street address)
        //     reflecting the start_location of this leg.
        public string StartAddress { get; set; }
        //
        // Summary:
        //     steps[] contains an array of steps denoting information about each separate
        //     step of the leg of the journey. (See Directions Steps below.)
        public virtual ICollection<Step> Steps { get; set; }
    }
}
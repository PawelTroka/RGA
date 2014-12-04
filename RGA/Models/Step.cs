using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RGA.Models
{
    public class Step
    {
        [Key]
        public string Id { get; set; }
        // Summary:
        //     distance contains the distance covered by this step until the next step.
        //     (See the discussion of this field in Directions Legs above.) This field may
        //     be undefined if the distance is unknown.
        public long Distance { get; set; }
        //
        // Summary:
        //     duration contains the typical time required to perform the step, until the
        //     next step (See the description in Directions Legs above.) This field may
        //     be undefined if the duration is unknown.
        public TimeSpan Duaration
        {
            get { return TimeSpan.FromTicks(DuarationTicks); }
            set { DuarationTicks = value.Ticks; }
        }

        public long DuarationTicks { get; set; }
        //
        // Summary:
        //     end_location contains the location of the starting point of this step, as
        //     a single set of lat and lng fields.
        public string EndLocation { get; set; }
        //
        // Summary:
        //     html_instructions contains formatted instructions for this step, presented
        //     as an HTML text string.
        [Display(Name = "Instrukcja tego kroku")]
        [DataType(DataType.Html)]
        public string HtmlInstructions { get; set; }
        //
        // Summary:
        //     start_location contains the location of the starting point of this step,
        //     as a single set of lat and lng fields.
        public string StartLocation { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RGA.Helpers
{
    public enum RouteOptimizationType
    {
        [Display(Name = "Czas trwania")]
        Time,
        [Display(Name = "Długość")]
        Distance
    }
}
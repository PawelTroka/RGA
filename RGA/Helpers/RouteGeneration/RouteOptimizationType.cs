using System.ComponentModel.DataAnnotations;

namespace RGA.Helpers
{
    public enum RouteOptimizationType
    {
        [Display(Name = "Czas trwania")] Time,
        [Display(Name = "Długość")] Distance
    }
}
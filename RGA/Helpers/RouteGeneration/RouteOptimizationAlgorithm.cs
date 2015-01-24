using System.ComponentModel.DataAnnotations;

namespace RGA.Helpers
{
    public enum RouteOptimizationAlgorithm
    {
        [Display(Name = "HeldKarp (zalecany)")] HeldKarp,
        [Display(Name = "BruteForce")] BruteForce
    }
}
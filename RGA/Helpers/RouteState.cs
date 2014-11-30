using System.ComponentModel.DataAnnotations;

namespace RGA.Helpers
{
    public enum RouteState
    {
        [Display(Name = "Nowa")]
        New,
        [Display(Name = "W trakcie")]
        InProgress,
        [Display(Name = "Ukończona")]
        Completed
    }
}

using System.ComponentModel.DataAnnotations;

namespace RGA.Helpers.RouteGeneration
{
    public enum DistanceMatrixProvider
    {
        [Display(Name = "GoogleMaps")]
        GoogleMaps,
        /* GoogleMaps
         * The Distance Matrix API has the following limits in place:
          * Users of the free API:
            100 elements per query.
            100 elements per 10 seconds.
            2 500 elements per 24 hour period.
          * Google Maps API for Work customers:
            625 elements per query.
            1 000 elements per 10 seconds.
            100 000 elements per 24 hour period */

        [Display(Name = "MapQuest (eksperymentalny)")]
        MapQuest, //http://www.mapquestapi.com/directions/
        /* MapQuest
         * Route Matrix Limits
            There are time and distance limits in place on the route matrix call because this can become a very expensive request. The route matrix is not intended for computing extremely long distances from a large number of locations.
            A One to Many route matrix call can handle up to 100 locations.
            A Many to One matrix call can handle up to 50 locations.
            An all to all route matrix call can handle up to 25 locations.
            Route matrix methods use what is called multi-destination path search. It expands from the origin location and marks each destination it finds. This search gets more expensive as the distance from the origin location increases, so the search is limited by a setting called MaxMatrixSearchTime. This is set to 180 minutes. Any destinations that lie outside this limit are found using regular "point to point" routes. However, the server limits the number of outlying locations (outside the MaxMatrixSearch limit) with a setting called MaxMatrixPointToPoint. This value is set to 25. Route matrix methods are intended to support many destinations within a short distance of each other. These limits allow several locations to be further away, but still protect the server from matrix requests that would take an exceptionally long time to compute. A user should break such requests into smaller sets to allow them to complete within the limits.*/


        [Display(Name = "BingMaps (wyłączony)")]
        BingMaps, //http://msdn.microsoft.com/en-us/library/ff701717.aspx


        [Display(Name = "OpenStreetMap (wyłączony)")]
        OpenStreetMap
    }
}
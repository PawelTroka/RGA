namespace RGA.Helpers
{
    public enum RouteOptimizationProvider
    {
        RGA,
        /* RGA
         * RGA can handle unlimited locations, that's why we created it :)
         */

        GoogleMaps,
        /* GoogleMaps
         * The Directions API has the following limits in place:
              * Users of the free API:
                 2,500 directions requests per 24 hour period.
                 Up to 8 waypoints allowed in each request. Waypoints are not available for transit directions.
                 2 requests per second.
              * Google Maps API for Work customers:
                 100,000 directions requests per 24 hour period.
                 23 waypoints allowed in each request. Waypoints are not available for transit directions.
                 10 requests per second.
         * Up to 8 waypoints allowed in each request
                 By default, the Directions service calculates a route through the provided waypoints in their given order. Optionally, you may pass optimize:true as the first argument within the waypoints parameter to allow the Directions service to optimize the provided route by rearranging the waypoints in a more efficient order. (This optimization is an application of the Travelling Salesman Problem.)
                 If you instruct the Directions service to optimize the order of its waypoints, their order will be returned in the waypoint_order field within the routes object. The waypoint_order field returns values which are zero-based.*/

        MapQuest,
        /* MapQuest
         * Optimized Route Options
            An Optimized Route request uses the "optimizedRoute" routing method. The request is sent with the Route Request parameters and finds the route with the most optimized drive time / shortest distance / walking time that includes all intermediate locations. The origin and destination locations remain fixed, but the intermediate locations are re-ordered to find the optimal path through the set of locations.
            The Optimized Route response is similar to a regular Route response except that in an Optimized Route response, the sort order among the intermediate locations may change and there is an additional response field called locationSequence that gets returned.
         * Optimized routes are limited to 25 locations.*/
        

        OpenStreetMap
    }
}
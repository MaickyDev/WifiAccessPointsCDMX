using WifiAccessPointsCDMX.Interfaces.Services;
using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.GraphQL
{
    public class Query
    {
        [UsePaging(IncludeTotalCount = true)] // Handles the "Total", "Items", and "Page" logic automatically
        [UseProjection]                      // Automatically Joins Alcaldia/Program only if requested
        [UseFiltering]                       // Optional: allows users to filter by Code, etc.
        [UseSorting]                         // Optional: allows users to sort results
        public IQueryable<AccessPointModel> GetAccessPoints([Service] IAccessPointService service)
        {
            return service.GetAccessPointsQuery();
        }

        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        public IQueryable<AccessPointModel> GetAccessPointsNearby(double lat, double lon, [Service] IAccessPointService service)
        {
            return service.GetAccessPointsQuery()
                .OrderBy(x => Math.Pow(x.Latitude - lat, 2) + Math.Pow(x.Longitude - lon, 2));
        }
    }
}

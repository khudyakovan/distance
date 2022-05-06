using System.Collections.Generic;

namespace distance.entity
{
    public class RouteQuery
    {
        public string type { get; set; }
        public string output { get; set; }
        public List<RoutePoint> points { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distance.entity
{
    internal class Office
    {
        public int ShopId { get; set; }
        public string OfficeLongitude { get; set; }
        public string OfficeLatitude { get; set; }

        public Office(int shopId, string officeLongitude, string officeLatitude)
        {
            ShopId = shopId;
            OfficeLongitude = officeLongitude;
            OfficeLatitude = officeLatitude;
        }

        public Office(int shopId)
        {
            ShopId = shopId;
        }
    }
}

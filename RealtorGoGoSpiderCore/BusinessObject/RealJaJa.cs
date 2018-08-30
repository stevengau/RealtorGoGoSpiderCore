using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorGoGoSpider.BusinessObject
{
    public class MapCoordinate
    {
        public MapCoordinate(double swLat, double swLng, double neLat, double neLng)
        {
            this.swLat = swLat;
            this.swLng = swLng;
            this.neLat = neLat;
            this.neLng = neLng;
        }
        public double swLat { get; set; }
        public double swLng { get; set; }
        public double neLat { get; set; }
        public double neLng { get; set; }
    }
    public class MapJaJa
    {
        public string _id { get; set; }
        public double lng { get; set; }
        public string price { get; set; }
        public string nodeId { get; set; }
        public double lat { get; set; }
        public string Html { get; set; }
        public string MongohouseData { get; set; }
    }

    public class Property
    {
        public string propertyName { get; set; }
        public string Value { get; set; }
    }

    public class RoomType
    {
        public string Type { get; set; }
        public string Level { get; set; }
        public string Dimensions { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
    }

    public class School
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public string Ranking { get; set; }
    }
    public class Picture
    {
        public string url { get; set; }
    }

    public class Twodsphere
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("coordinates")]
        public IList<double> coordinates { get; set; }

    }

    public class PropertyDetailByJaJa : IForMap
    {
        public string _id { get; set; }
        public string JmlsNo { get; set; }
        public string MlsNo { get; set; }
        public string Address { get; set; }
        public string Bedrooms { get; set; }
        public string Washrooms { get; set; }
        public string Garage { get; set; }
        public string Footage { get; set; }
        public string HouseType { get; set; }
        public string ListingDate { get; set; }
        public string ListingPrice { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionPrice { get; set; }
        public string LastStatus { get; set; }
        public string City { get; set; }
        public string Community { get; set; }
        public string District { get; set; }
        public string MCity { get; set; }
        public string MAddress1 { get; set; }
        public string MAddress2 { get; set; }
        [JsonProperty("Location")]
        public Twodsphere Location { get; set; }

        public List<Property> Properties { get; set; }
        public List<RoomType> RoomTypes { get; set; }
        public List<School> Schools { get; set; }
        public List<Picture> Pictures { get; set; }

        public bool Active { get; set; }

        public String UpdateDate { get; set; }
    }

    public interface IForMap
    {
        string _id { get; set; }
        string MlsNo { get; set; }
        [JsonProperty("Location")]
        Twodsphere Location { get; set; }

        bool Active { get; set; }
    }

    public class ForMap: IForMap
    {
        public string _id { get; set; }
        public string MlsNo { get; set; }
        [JsonProperty("Location")]
        public Twodsphere Location { get; set; }

        public bool Active { get; set; }
    }
}

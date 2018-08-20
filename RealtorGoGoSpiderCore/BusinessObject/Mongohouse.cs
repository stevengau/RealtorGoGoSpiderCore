using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorGoGoSpider.BusinessObject
{
    public class MapData
    {
        public MapData(string id ,Twodsphere sphere, string mlsNo)
        {
            this.Id = id;
            this.Twodsphere = sphere;
            this.MLSNo = mlsNo;

        }
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_2dsphere")]
        public Twodsphere Twodsphere { get; set; }

        [JsonProperty("MLS#")]
        public string MLSNo { get; set; }
    }

    public class SearchUrl
    {
        [JsonProperty("_id")]
        public string _id { get; set; }
        public string Url { get; set; }
        public string ExecutionDate { get; set; }
        public string QueryResult { get; set; }
    }


    public class MongoHousePriceHistory
    {

        [JsonProperty("_id")]
        public string _id { get; set; }

        [JsonProperty("_Sold_Date")]
        public DateTime SoldDate { get; set; }

        [JsonProperty("List")]
        public string List { get; set; }

        [JsonProperty("Last Status")]
        public string LastStatus { get; set; }

        [JsonProperty("MLS#")]
        public string MLSNo { get; set; }

        [JsonProperty("_Contract_Date")]
        public DateTime ContractDate { get; set; }

        [JsonProperty("Sold")]
        public string Sold { get; set; }

        [JsonProperty("_count")]
        public int Count { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

    }

    public class PropertyContractResolver : DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        public PropertyContractResolver()
        {
            this.PropertyMappings = new Dictionary<string, string>
        {
            {"Id", "_id"},
            {"ContractDate", "Contract Date"},
            {"Informations", "Undefined"},
            {"LastStatus", "Last Status"},
            {"MLSNo", "MLS#"},
            {"Active", "_active"},
            {"openhouses", "openhouses"},
            {"SoldDate", "Sold Date"},
        };
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }

    public class MapContractResolver : DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        public MapContractResolver()
        {
            this.PropertyMappings = new Dictionary<string, string>
        {
            {"Id", "_id"},
            {"ContractDate", "Contract Date"},
            {"Informations", "Undefined"},
            {"LastStatus", "Last Status"},
            {"MLSNo", "MLS#"},
            {"Active", "_active"},
            {"openhouses", "openhouses"},
            {"SoldDate", "Sold Date"},
        };
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }



    public class Openhouse
    {

        public string From { get; set; }

        public string Id { get; set; }

        public string To { get; set; }

        public DateTime Date { get; set; }
    }

    public class MongohouseProperty
    {

        public string _id { get; set; }

        public string City { get; set; }

        public string Washrooms { get; set; }

        public string For { get; set; }

        public IList<Picture> Pictures { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Bedrooms { get; set; }

        public DateTime? ContractDate { get; set; }

        public IList<string> Informations { get; set; }

        public string List { get; set; }

        public string LastStatus { get; set; }

        public string MLSNo { get; set; }

        public bool Active { get; set; }

        public IList<Openhouse> Openhouses { get; set; }

        public string Description { get; set; }

        public DateTime? SoldDate { get; set; }

        public string Sold { get; set; }

        public string Type { get; set; }
    }



    public class MongohouseMapProperty
    {

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_List")]
        public int List { get; set; }

        [JsonProperty("For")]
        public string For { get; set; }

        [JsonProperty("_2dsphere")]
        public Twodsphere Twodsphere { get; set; }

        [JsonProperty("Last Status")]
        public string LastStatus { get; set; }

        [JsonProperty("MLS#")]
        public string MLSNo { get; set; }
    }

    public class BungolMapResult
    {

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("mls_number")]
        public string mls_number { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("la")]
        public string la { get; set; }

        [JsonProperty("lo")]
        public string lo { get; set; }
    }

    public class BungolMapProperty
    {

        [JsonProperty("results")]
        public IList<BungolMapResult> results { get; set; }
    }

    public class List
    {

        [JsonProperty("Value")]
        public int Value { get; set; }
    }

    public class History
    {

        [JsonProperty("_List")]
        public List _List { get; set; }
    }

    public class HistoricalResult
    {

        [JsonProperty("_id")]
        public string _id { get; set; }

        [JsonProperty("_History")]
        public IList<History> History { get; set; }

        [JsonProperty("List")]
        public string List { get; set; }

        [JsonProperty("Last Status")]
        public string LastStatus { get; set; }

        [JsonProperty("MLS#")]
        public string MLSNo { get; set; }

        [JsonProperty("_count")]
        public int Count { get; set; }

        [JsonProperty("_Contract_Date")]
        public DateTime ContractDate { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }
    }
}

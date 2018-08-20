using HtmlAgilityPack;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealtorGoGoSpider.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorGoGoSpider
{
    public class Mongohouse
    {
        static HashSet<SearchUrl> mongohouseSoldUrls = new HashSet<SearchUrl>();
        public static void GenerateMongohouseSoldUrl(string days)
        {
            Console.WriteLine("******GenerateMongohouseSoldUrl******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<BsonDocument> collection = mongoDatabase.GetCollection<BsonDocument>("MongoHouseSoldUrls");

            for (double x = 41; x <= 47; x = x + 0.05)
            {
                for (double y = -83; y <= -73; y = y + 0.05)
                {
                    //mongohouseSoldUrls.Add(new SearchUrl() { _id = ObjectId.GenerateNewId().ToString(), Url = SoldListingUrl(x, y, x + 0.05, y + 0.05, days), ExecutionDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"), QueryResult = "0" });
                    var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(new SearchUrl() { _id = ObjectId.GenerateNewId().ToString(), Url = SoldListingUrl(x, y, x + 0.05, y + 0.05, days), ExecutionDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"), QueryResult = "0" }));
                    collection.InsertOne(bsonDoc);
                }
            }
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }

        public static void GenerateWaitSoldProcessing(string updateDate)
        {
            Console.WriteLine("******GenerateWaitSoldProcessing******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<SearchUrl> collection = mongoDatabase.GetCollection<SearchUrl>("MongoHouseSoldUrls");

            List<SearchUrl> documents = collection.Find(_ => true).ToList();
            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 50 },
                       doc => { ExececuteSoldUrl(doc.Url); }
                     );
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }


        public static void ExececuteSoldUrl(string url)
        {
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<BsonDocument> collection = mongoDatabase.GetCollection<BsonDocument>("SaleMap");

            string responseContent = OwnHttpClient.GetResponseContent(url).Result;

            int count = 0;
            try
            {
                if (responseContent != "[]")
                {
                    List<MongohouseMapProperty> mongoHouseMaps = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MongohouseMapProperty>>(
                        responseContent, new JsonSerializerSettings() { ContractResolver = new MapContractResolver() });
                    count = mongoHouseMaps.Count;
                    foreach (MongohouseMapProperty x in mongoHouseMaps)
                    {
                        MapJaJa map = new MapJaJa()
                        {
                            _id = ObjectId.GenerateNewId().ToString(),
                            nodeId = "C-" + x.MLSNo,
                            lat = x.Twodsphere.coordinates[0],
                            lng = x.Twodsphere.coordinates[1]
                        };
                        IMongoCollection<BsonDocument> pcollection = mongoDatabase.GetCollection<BsonDocument>("WaitProcessingForSoldJaJa");


                        var filter = new BsonDocument("nodeId", map.nodeId);
                        var FindResult = pcollection.Find(filter);
                        var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(map));
                        if (FindResult.CountDocuments() == 0)
                        {
                            pcollection.InsertOne(bsonDoc);
                        }
                    }

                }



                IMongoCollection<BsonDocument> exeCollection = mongoDatabase.GetCollection<BsonDocument>("MongoHouseSoldUrls");
                var mfilter = new BsonDocument("Url", url);
                List<BsonDocument> bdocs = exeCollection.Find(mfilter).ToList();
                foreach (BsonDocument doc in bdocs)
                {
                    doc["QueryResult"] = doc["QueryResult"] + "-" + count.ToString();
                    exeCollection.ReplaceOne(mfilter, doc);
                    System.Diagnostics.Debug.WriteLine(doc["Url"].ToString());
                }

            }
            catch (Exception) { }


        }

        public static void GetWaitSoldProcessing(string updateDate)
        {
            Console.WriteLine("******GetWaitSoldProcessing******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSoldJaJa");

            List<MapJaJa> documents = collection.Find(_ => true).ToList();

            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 50 },
                       doc => { GetSoldHtml(doc, updateDate); }
                     );


            // OwnThread.StartAndWaitAllThrottled(listOfTasks, 3);
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }

        public static void GetWaitSoldHTMLProcessing(string updateDate)
        {
            Console.WriteLine("******GetWaitSoldHTMLProcessing******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSoldJaJaHTML");

            List<MapJaJa> documents = collection.Find(_ => true).ToList();

            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 200 },
                       doc => { GetSoldData(doc, updateDate); }
                     );
            collection.DeleteMany(_ => true);
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }


        public static void GetSoldHtml(MapJaJa mls, string updateDate)
        {
            mls.Html = RealJaJa.Details(mls.nodeId);
            mls.MongohouseData = MLSSearch(mls.nodeId.Substring(2));
            #region Save to MongoDB
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSoldJaJa");

            IMongoCollection<MapJaJa> pcollection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSoldJaJaHTML");

            collection.DeleteOne(c => c.nodeId == mls.nodeId);
            if (mls.MongohouseData.Trim() != "{}")
                pcollection.InsertOne(mls);

            #endregion
        }


        public static void GetSoldData(MapJaJa mls, string updateDate)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(mls.Html);

                PropertyDetailByJaJa houseDetail = new PropertyDetailByJaJa();

                #region Data Processing
                houseDetail._id = ObjectId.GenerateNewId().ToString();
                houseDetail.JmlsNo = mls.nodeId;

                List<HtmlNode> mapLocation = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "text-white").ToList<HtmlNode>();

                houseDetail.Location = new Twodsphere();

                houseDetail.Location = new Twodsphere() { type = "Point", coordinates = new List<double>() { mls.lat, mls.lng } };
                houseDetail.UpdateDate = updateDate;

                // Address
                List<HtmlNode> divAddress = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "div-address").ToList<HtmlNode>();
                if (divAddress.Count > 0)
                {
                    houseDetail.Address = divAddress[0].InnerText.Replace("\n", "").Trim();
                }
                // Price
                List<HtmlNode> divPrice = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "text-ui").ToList<HtmlNode>();
                if (divPrice.Count > 0)
                {
                    houseDetail.ListingPrice = divPrice[0].InnerText.Replace("CAD", "").Replace("$", "").Replace(",", "").Trim();
                }
                List<HtmlNode> divBedroom = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "fa-bed").ToList<HtmlNode>();


                List<HtmlNode> divDetail = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "div-detail").ToList<HtmlNode>();
                if (divDetail.Count == 0)
                    return;
                List<HtmlNode> properties = HtmlParserHelper.GetElementsWithClass(divDetail[0], "col-xs-6").ToList<HtmlNode>();
                houseDetail.Properties = new List<Property>();

                for (int i = 0; i < properties.Count; i = i + 2)
                {
                    houseDetail.Properties.Add(new Property() { propertyName = properties[i].InnerText.Replace("\n", "").Trim(), Value = properties[i + 1].InnerText.Replace("\n", "").Trim() });
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("City"))
                        houseDetail.City = properties[i + 1].InnerText.Replace("\n", "").Trim();
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("Community"))
                        houseDetail.Community = properties[i + 1].InnerText.Replace("\n", "").Trim();
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("District"))
                        houseDetail.District = properties[i + 1].InnerText.Replace("\n", "").Trim();
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("MLS"))
                        houseDetail.MlsNo = properties[i + 1].InnerText.Replace("\n", "").Trim();
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("Bed Rooms"))
                        houseDetail.Bedrooms = properties[i + 1].InnerText.Replace("\n", "").Trim().Replace(" ", "");
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("Bath Rooms"))
                        houseDetail.Washrooms = properties[i + 1].InnerText.Replace("\n", "").Trim().Replace(" ", "");
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("Gar Spaces"))
                        houseDetail.Garage = properties[i + 1].InnerText.Replace("\n", "").Trim();
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("Listed Date"))
                        houseDetail.ListingDate = properties[i + 1].InnerText.Replace("\n", "").Trim();
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("Delisted Date"))
                        houseDetail.TransactionDate = properties[i + 1].InnerText.Replace("\n", "").Trim();
                    if (properties[i].InnerText.Replace("\n", "").Trim().StartsWith("Appr. Footage"))
                        houseDetail.Footage = properties[i + 1].InnerText.Replace("\n", "").Trim();

                }

                if (houseDetail.MlsNo == null)
                    return;
                List<HtmlNode> Type = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "rm_out").ToList<HtmlNode>();
                List<HtmlNode> Level = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "rm_lvl").ToList<HtmlNode>();
                List<HtmlNode> Dimensions = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "rm_dem").ToList<HtmlNode>();
                List<HtmlNode> Desc1 = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "rm_dc1").ToList<HtmlNode>();
                List<HtmlNode> Desc2 = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "rm_dc2").ToList<HtmlNode>();
                List<HtmlNode> Desc3 = HtmlParserHelper.GetElementsWithClass(htmlDoc.DocumentNode, "rm_dc3").ToList<HtmlNode>();
                houseDetail.RoomTypes = new List<RoomType>();
                for (int i = 0; i < Type.Count; i = i + 1)
                {
                    houseDetail.RoomTypes.Add(new RoomType()
                    {
                        Type = Type[i].InnerText.Replace("\n", "").Trim(),
                        Level = Level[i].InnerText.Replace("\n", "").Trim(),
                        Dimensions = Dimensions[i].InnerText.Replace("\n", "").Trim(),
                        Desc1 = Desc1[i].InnerText.Replace("\n", "").Trim(),
                        Desc2 = Desc2[i].InnerText.Replace("\n", "").Trim(),
                        Desc3 = Desc3[i].InnerText.Replace("\n", "").Trim(),
                    });
                }

                if (houseDetail.RoomTypes.Count > 0) houseDetail.RoomTypes.RemoveAt(0);
                List<HtmlNode> schools = HtmlParserHelper.GetElementsByID(htmlDoc.DocumentNode, "school_collapse").ToList<HtmlNode>();
                if (schools.Count > 0)
                {
                    List<HtmlNode> schoolMaster = HtmlParserHelper.GetElementsWithClass(schools[0], "col-xs-9").ToList<HtmlNode>();
                    List<HtmlNode> schoolRank = HtmlParserHelper.GetElementsWithClass(schools[0], "col-xs-3").ToList<HtmlNode>();
                    houseDetail.Schools = new List<School>();
                    for (int i = 0; i < schoolMaster.Count; i = i + 1)
                    {
                        string name = schoolMaster[i].ChildNodes[1].InnerText.Replace("\n", "").Trim();
                        string detail = schoolMaster[i].ChildNodes[3].InnerText.Replace("\n", "").Trim();
                        string rank = schoolRank[i].InnerText.Replace("\n", "").Trim();
                        houseDetail.Schools.Add(new School() { Name = name, Detail = detail, Ranking = rank });
                    }
                }
                List<HtmlNode> picturesSection = HtmlParserHelper.GetElementsByID(htmlDoc.DocumentNode, "swiper_gallery").ToList<HtmlNode>();
                if (picturesSection.Count == 0)
                    return;

                List<HtmlNode> pictures = HtmlParserHelper.GetElementsByElementType(picturesSection[0], "img").ToList<HtmlNode>();
                houseDetail.Pictures = new List<Picture>();
                for (int i = 0; i < pictures.Count; i = i + 1)
                {
                    string name = pictures[i].GetAttributeValue("src", null);
                    if (name != null)
                    {
                        name = name.Substring(name.LastIndexOf("/"));
                        name = name.Substring(1);
                        if (name.LastIndexOf("-") > 0)
                            name = name.Substring(0, name.LastIndexOf("-"));
                        if (name.LastIndexOf("_") > 0)
                            name = name.Substring(0, name.LastIndexOf("_"));
                        name = string.Format("http://photos.v3.torontomls.net/Live/photos/FULL/{0}/{1}/{2}{3}.jpg",
                            i + 1, houseDetail.MlsNo.Substring(houseDetail.MlsNo.Length - 3), houseDetail.MlsNo, (i == 0) ? "" : "_" + (i + 1).ToString());
                        houseDetail.Pictures.Add(new Picture() { url = name });
                    }
                }
                #endregion
                // MOngohouse Detail
                try
                {
                    string rawJson = mls.MongohouseData;
                    dynamic task = JObject.Parse(rawJson);
                    houseDetail.LastStatus = task["Last Status"];
                    houseDetail.TransactionDate = task["Sold Date"];
                    houseDetail.TransactionPrice = task["Sold"];
                    houseDetail.TransactionPrice = houseDetail.TransactionPrice.Replace("$", "").Replace(",", "");
                    houseDetail.MCity = task["City"];
                    houseDetail.MAddress1 = task["Address1"];
                    houseDetail.MAddress2 = task["Address2"];
                    //houseDetail.Active = task["_active"];
                    MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
                    IMongoDatabase db = mongoClient.GetDatabase("RealEstate");
                    IMongoCollection<PropertyDetailByJaJa> collection = db.GetCollection<PropertyDetailByJaJa>("ForSold");


                    var obj = collection.Find(c => c.MlsNo == houseDetail.MlsNo).FirstOrDefault();


                    if (obj == null)
                    {
                        collection.InsertOne(houseDetail);
                    }
                    else
                    {
                        houseDetail._id = obj._id;
                        collection.ReplaceOne(c => c.MlsNo == houseDetail.MlsNo, houseDetail);
                    }



                    IMongoCollection<MapJaJa> mapcollection = db.GetCollection<MapJaJa>("WaitProcessingForSoldJaJaHTML");
                    var mfilter = new BsonDocument("nodeId", mls.nodeId);
                    mapcollection.DeleteOne(mfilter);

                }
                catch (Exception )
                {

                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        public static string MLSSearch(string mls)
        {
            string url = string.Format("https://mongohouse.com/api/quick_search_by_mls?token={0}", mls);

            return OwnHttpClient.GetResponseContent(url).Result;
        }


        public static string SoldListingUrl(double south, double west, double north, double east, string days)
        {
            string url = string.Format("https://mongohouse.com/api/soldrecords?query=true&price_min=$0&price_max=$999,999,999&sold_day_back={4}&detached=false&semi=false&condo=false&town=false&bedrooms=0&washrooms=0&ownershiptype=all&south={0}&west={1}&north={2}&east={3}&_2dsphere=true", south, west, north, east, days);
            return url;
        }

        public static string HistoricalUrl(string address1, string address2, string city)
        {
            string url = string.Format("https://mongohouse.com/api/historical_prices_sale?Address1={0}&Address2={1}&City={2}", address1, address2, city);

            return OwnHttpClient.GetResponseContent(url).Result;
        }

        public static List<HistoricalResult> HistoricalResult(string address1, string address2, string city)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoricalResult>>(HistoricalUrl(address1,address2,city));
        }

        public static void GetRecordToUpdateHistoricalTransaction(string updateDate)
        {
            Console.WriteLine("******GetRecordToUpdateHistoricalTransaction******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<PropertyDetailByJaJa> collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForSale");

            List<PropertyDetailByJaJa> documents = collection.Find(_ => true).ToList();

            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 20 },
                       doc => { UpdateHistoricalTransaction(doc.MAddress1, doc.MAddress2, doc.MCity); }
                     );

            collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForSold");

            documents = collection.Find(_ => true).ToList();

            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 20 },
                       doc => { UpdateHistoricalTransaction(doc.MAddress1, doc.MAddress2, doc.MCity); }
                     );

            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");

          
        }

        public static void UpdateHistoricalTransaction(string address1, string address2, string city)
        {
           
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<HistoricalResult> collection = mongoDatabase.GetCollection<HistoricalResult>("SoldHistory");

            
            List<HistoricalResult> mongoResult = Mongohouse.HistoricalResult(address1, address2, city);
            
            foreach(HistoricalResult o in mongoResult)
            {
                if (collection.Find(c=>c.MLSNo == o.MLSNo).CountDocuments() > 0)
                {

                }
                else
                {
                    o.Address1 = address1;
                    o.Address2 = address2;
                    o.City = city;
                    collection.InsertOne(o);
                }

            }
           
        }

        public static void UpdateMapData(string updateDate)
        {
            Console.WriteLine("******GetRecordToUpdateMapData******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");


            IMongoCollection<PropertyDetailByJaJa> collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForSale");

            IMongoCollection<MapData> maps = mongoDatabase.GetCollection<MapData>("ForSaleMap");

            List<PropertyDetailByJaJa> documents = collection.Find(_ => true).ToList();
            

            foreach(PropertyDetailByJaJa doc in documents)
            {
                MapData map = new MapData(ObjectId.GenerateNewId().ToString() ,doc.Location, doc.MlsNo);
                if (maps.Find(c => c.MLSNo == doc.MlsNo).CountDocuments() > 0)
                {
                    maps.DeleteOne(c => c.MLSNo == doc.MlsNo);
                    maps.InsertOne(map);
                }
                else
                    maps.InsertOne(map);
            }

            collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForLease");

            maps = mongoDatabase.GetCollection<MapData>("ForLeaseMap");

            documents = collection.Find(_ => true).ToList();
            foreach (PropertyDetailByJaJa doc in documents)
            {
                MapData map = new MapData(ObjectId.GenerateNewId().ToString(),doc.Location, doc.MlsNo);
                if (maps.Find(c => c.MLSNo == doc.MlsNo).CountDocuments() > 0)
                {
                    maps.DeleteOne(c => c.MLSNo == doc.MlsNo);
                    maps.InsertOne(map);
                }
                else
                    maps.InsertOne(map);
            }

            collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForSold");

            maps = mongoDatabase.GetCollection<MapData>("ForSoldMap");

            documents = collection.Find(_ => true).ToList();

            foreach (PropertyDetailByJaJa doc in documents)
            {
                MapData map = new MapData(ObjectId.GenerateNewId().ToString(),doc.Location, doc.MlsNo);
                if (maps.Find(c => c.MLSNo == doc.MlsNo).CountDocuments() > 0)
                {
                    maps.DeleteOne(c => c.MLSNo == doc.MlsNo);
                    maps.InsertOne(map);
                }
                else
                    maps.InsertOne(map);
            }



            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");


        }

    }
}

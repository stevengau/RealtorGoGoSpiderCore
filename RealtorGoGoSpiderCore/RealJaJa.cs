using HtmlAgilityPack;
using MongoDB.Bson;
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
    public class RealJaJa
    {
        static HashSet<MapJaJa> MlsNos = new HashSet<MapJaJa>();

        public static void RealJaJaForSale(string updateDate)
        {
            Console.WriteLine("******RealJaJaForLease******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);

            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");

            mongoDatabase.DropCollection("WaitProcessingForSaleJaJa");

            RealJajaForSaleMap(41, -83, 47, -73, 1);
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");

        }

        public static void RealJaJaForLease(string updateDate)
        {
            Console.WriteLine("******RealJaJaForLease******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);

            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");

            mongoDatabase.DropCollection("WaitProcessingForLeaseJaJa");

            RealJajaForLeaseMap(41, -83, 47, -73, 1);
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");

        }

        public static void GetWaitSaleProcessing(string updateDate)
        {
            Console.WriteLine("******GetWaitSaleProcessing******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSaleJaJa");

            List<MapJaJa> documents = collection.Find(_ => true).ToList();

            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 50 },
                       doc => { GetSaleHtml(doc, updateDate); }
                     );


            // OwnThread.StartAndWaitAllThrottled(listOfTasks, 3);
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }

        public static void GetWaitLeaseProcessing(string updateDate)
        {
            Console.WriteLine("******GetWaitLeaseProcessing******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForLeaseJaJa");

            List<MapJaJa> documents = collection.Find(_ => true).ToList();
            
            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 10 },
                       doc => { GetLeaseHtml(doc, updateDate); }
                     );


            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }

        public static void GetWaitLeaseHTMLProcessing(string updateDate)
        {
            Console.WriteLine("******GetWaitLeaseHTMLProcessing******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForLeaseJaJaHTML");

            List<MapJaJa> documents = collection.Find(_ => true).ToList();

            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 200 },
                       doc => { GetLeaseData(doc, updateDate); }
                     );
            collection.DeleteMany(_ => true);
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }

        public static void GetWaitSaleHTMLProcessing(string updateDate)
        {
            Console.WriteLine("******GetWaitSaleHTMLProcessing******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSaleJaJaHTML");

            List<MapJaJa> documents = collection.Find(_ => true).ToList();

            Parallel.ForEach(
                       documents,
                       new ParallelOptions { MaxDegreeOfParallelism = 200 },
                       doc => { GetSaleData(doc, updateDate); }
                     );
            collection.DeleteMany(_ => true);
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");
        }


        public static void DeavtiveNotRenewLeaseInformation(string updateDate)
        {
            Console.WriteLine("******DeavtiveNotRenewLeaseInformation******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<PropertyDetailByJaJa> collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForLease");

            List<PropertyDetailByJaJa> lc = collection.Find(c=>c.UpdateDate != updateDate).ToList();

            foreach (PropertyDetailByJaJa o in lc)
            {
                o.Active = false;
                collection.ReplaceOne(c => c.MlsNo == o.MlsNo, o);
            }
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");


        }

        public static void DeavtiveNotRenewSaleInformation(string updateDate)
        {
            Console.WriteLine("******DeavtiveNotRenewLeaseInformation******");
            Console.WriteLine("Start DateTime: {0}", DateTime.Now);
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<PropertyDetailByJaJa> collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForSale");

            List<PropertyDetailByJaJa> lc = collection.Find(c => c.UpdateDate != updateDate).ToList();

            foreach (PropertyDetailByJaJa o in lc)
            {
                o.Active = false;
                collection.ReplaceOne(c => c.MlsNo == o.MlsNo, o);
            }
            Console.WriteLine("End   DateTime: {0}", DateTime.Now);
            Console.WriteLine("*********************************************");


        }


        public static void RealJajaForSaleMap(double swLat, double swLng, double neLat, double neLng, double increment)
        {

            System.Threading.Thread.Sleep(500);
            string result = ForSale(swLat, swLng, neLat, neLng);
            List<MapJaJa> mapDatas = JsonConvert.DeserializeObject<List<MapJaJa>>(result);

            if (mapDatas.Count >= 300)
            {
                List<MapCoordinate> cords = new List<MapCoordinate>();
                cords.Add(new MapCoordinate(swLat, swLng, (swLat + neLat) / 2, (swLng + neLng) / 2));
                cords.Add(new MapCoordinate(swLat, (swLng + neLng) / 2, (swLat + neLat) / 2, neLng));
                cords.Add(new MapCoordinate((swLat + neLat) / 2, swLng, neLat, (neLng + swLng) / 2));
                cords.Add(new MapCoordinate((swLat + neLat) / 2, (swLng + swLng) / 2, neLat, neLng));
                Parallel.ForEach(
                       cords,
                       new ParallelOptions { MaxDegreeOfParallelism = 4 },
                       cord => { RealJajaForSaleMap(cord.swLat, cord.swLng, cord.neLat, cord.neLng, 0); }
                     );
            }
            else
            {
                foreach (MapJaJa map in mapDatas)
                {
                    char c = map.nodeId.Substring(2, 1).ToCharArray()[0];
                    if ((c >= 'A' && c <= 'Z'))
                    {
                        map._id = ObjectId.GenerateNewId().ToString();
                        MlsNos.Add(map);

                        #region Save to MongoDB
                        MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
                        IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
                        IMongoCollection<BsonDocument> collection = mongoDatabase.GetCollection<BsonDocument>("WaitProcessingForSaleJaJa");


                        var filter = new BsonDocument("nodeId", map.nodeId);
                        var FindResult = collection.Find(filter);
                        var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(JsonConvert.SerializeObject(map));
                        if (FindResult.CountDocuments() == 0)
                        {
                            collection.InsertOne(bsonDoc);
                        }
                        #endregion
                    }
                }
            }
        }

        public static void RealJajaForLeaseMap(double swLat, double swLng, double neLat, double neLng, double increment)
        {

            System.Threading.Thread.Sleep(500);
            string result = ForLease(swLat, swLng, neLat, neLng);
            List<MapJaJa> mapDatas = JsonConvert.DeserializeObject<List<MapJaJa>>(result);

            if (mapDatas.Count >= 300)
            {
                List<MapCoordinate> cords = new List<MapCoordinate>();
                cords.Add(new MapCoordinate(swLat, swLng, (swLat + neLat) / 2, (swLng + neLng) / 2) );
                cords.Add(new MapCoordinate(swLat, (swLng + neLng) / 2, (swLat + neLat) / 2, neLng));
                cords.Add(new MapCoordinate((swLat + neLat) / 2, swLng, neLat, (neLng + swLng) / 2));
                cords.Add(new MapCoordinate((swLat + neLat) / 2, (swLng + swLng) / 2, neLat, neLng));
                Parallel.ForEach(
                       cords,
                       new ParallelOptions { MaxDegreeOfParallelism = 4 },
                       cord => { RealJajaForLeaseMap(cord.swLat, cord.swLng, cord.neLat, cord.neLng, 0); }
                     );
            }
            else
            {
                foreach (MapJaJa map in mapDatas)
                {
                    char c = map.nodeId.Substring(2, 1).ToCharArray()[0];
                    if ((c >= 'A' && c <= 'Z'))
                    {
                        map._id = ObjectId.GenerateNewId().ToString();
                        MlsNos.Add(map);

                        #region Save to MongoDB
                        MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
                        IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
                        IMongoCollection<BsonDocument> collection = mongoDatabase.GetCollection<BsonDocument>("WaitProcessingForLeaseJaJa");


                        var filter = new BsonDocument("nodeId", map.nodeId);
                        var FindResult = collection.Find(filter);
                        var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(JsonConvert.SerializeObject(map));
                        if (FindResult.CountDocuments() == 0)
                        {
                            collection.InsertOne(bsonDoc);
                        }
                        #endregion
                    }
                }
            }
        }

        public static void GetLeaseHtml(MapJaJa mls, string updateDate)
        {
            mls.Html = Details(mls.nodeId);
            #region Save to MongoDB
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForLeaseJaJa");

            IMongoCollection<MapJaJa> pcollection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForLeaseJaJaHTML");

            //var FindResult = collection.Find(c => c.nodeId == mls.nodeId).First();
            collection.DeleteOne(c => c.nodeId == mls.nodeId);
            pcollection.InsertOne(mls);
            
            #endregion
        }


        public static void GetSaleHtml(MapJaJa mls, string updateDate)
        {
            mls.Html = Details(mls.nodeId);
            mls.MongohouseData = Mongohouse.MLSSearch(mls.nodeId.Substring(2));
            #region Save to MongoDB
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            IMongoCollection<MapJaJa> collection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSaleJaJa");

            IMongoCollection<MapJaJa> pcollection = mongoDatabase.GetCollection<MapJaJa>("WaitProcessingForSaleJaJaHTML");

            //var FindResult = collection.Find(c => c.nodeId == mls.nodeId).First();
            collection.DeleteOne(c => c.nodeId == mls.nodeId);
            pcollection.InsertOne(mls);

            #endregion
        }



        public static void GetLeaseData(MapJaJa mls, string updateDate)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(mls.Html);

                PropertyDetailByJaJa houseDetail = new PropertyDetailByJaJa();
                houseDetail.Active = true;
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

                MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
                IMongoDatabase db = mongoClient.GetDatabase("RealEstate");
                IMongoCollection<PropertyDetailByJaJa> collection = db.GetCollection<PropertyDetailByJaJa>("ForLease");


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

                IMongoCollection<MapJaJa> mapcollection = db.GetCollection<MapJaJa>("WaitProcessingForLeaseJaJaHTML");
                var mfilter = new BsonDocument("nodeId", mls.nodeId);
                mapcollection.DeleteOne(mfilter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        public static void GetSaleData(MapJaJa mls, string updateDate)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(mls.Html);

                PropertyDetailByJaJa houseDetail = new PropertyDetailByJaJa();
                MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
                IMongoDatabase db = mongoClient.GetDatabase("RealEstate");

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

                   
                }
                catch (Exception )
                {

                }

                IMongoCollection<PropertyDetailByJaJa> collection = db.GetCollection<PropertyDetailByJaJa>("ForSale");


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

                IMongoCollection<MapJaJa> mapcollection = db.GetCollection<MapJaJa>("WaitProcessingForSaleJaJaHTML");
                var mfilter = new BsonDocument("nodeId", mls.nodeId);
                mapcollection.DeleteOne(mfilter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        public static string ForSale(double swLat, double swLng, double neLat, double neLng)
        {
            string url = string.Format("http://www.realsforce.com/playserver/getNearByListings?swLat={0}&swLng={1}&neLat={2}&neLng={3}&accountId=10749&listingtype=Sale&mlsnum=undefined", swLat, swLng, neLat, neLng);

            return OwnHttpClient.GetResponseContent(url).Result;

        }

        public static string ForLease(double swLat, double swLng, double neLat, double neLng)
        {
            string url = string.Format("http://www.realsforce.com/playserver/getNearByListings?swLat={0}&swLng={1}&neLat={2}&neLng={3}&accountId=10749&listingtype=Lease&mlsnum=undefined", swLat, swLng, neLat, neLng);

            return OwnHttpClient.GetResponseContent(url).Result;
           

        }

        public static string Details(string mls)
        {
            string url = string.Format("http://www.realsforce.com/en/fd/listing/{0}", mls);

            return OwnHttpClient.GetResponseContent(url).Result;

        }
    }
}

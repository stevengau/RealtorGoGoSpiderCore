using System;

namespace RealtorGoGoSpiderCore
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("******Start Lease Process******");
            RealtorGoGoSpider.RealJaJa.RealJaJaForLease(now.ToString("yyyy-MM-dd"));

            RealtorGoGoSpider.RealJaJa.GetWaitLeaseProcessing(now.ToString("yyyy-MM-dd"));

            RealtorGoGoSpider.RealJaJa.GetWaitLeaseHTMLProcessing(now.ToString("yyyy-MM-dd"));

            RealtorGoGoSpider.RealJaJa.DeavtiveNotRenewLeaseInformation(now.ToString("yyyy-MM-dd"));

            //Console.WriteLine("******Start Sale Process******");
            //RealtorGoGoSpider.RealJaJa.RealJaJaForSale(now.ToString("yyyy-MM-dd"));

            //RealtorGoGoSpider.RealJaJa.GetWaitSaleProcessing(now.ToString("yyyy-MM-dd"));

            //RealtorGoGoSpider.RealJaJa.GetWaitSaleHTMLProcessing(now.ToString("yyyy-MM-dd"));

            //RealtorGoGoSpider.RealJaJa.DeavtiveNotRenewSaleInformation(now.ToString("yyyy-MM-dd"));

            //Console.WriteLine("******Start Sold Process******");
            //RealtorGoGoSpider.Mongohouse.GenerateMongohouseSoldUrl("30");

            //RealtorGoGoSpider.Mongohouse.GenerateWaitSoldProcessing(new DateTime(2018, 8, 1).ToString("yyyy-MM-dd"));

            //RealtorGoGoSpider.Mongohouse.GetWaitSoldProcessing(new DateTime(2018, 8, 1).ToString("yyyy-MM-dd"));

            //RealtorGoGoSpider.Mongohouse.GetWaitSoldHTMLProcessing(now.ToString("yyyy-MM-dd"));

            //Console.WriteLine("******Update Historical Data******");

            RealtorGoGoSpider.Mongohouse.GetRecordToUpdateHistoricalTransaction(now.ToString("yyyy-MM-dd"));

            RealtorGoGoSpider.Mongohouse.UpdateMapData(now.ToString("yyyy-MM-dd"));

            Console.ReadLine();
        }
    }
}

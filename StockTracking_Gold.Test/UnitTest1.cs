using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTracking_Gold.Ap.Applibs;
using StockTracking_Gold.Ap.Dto;
using StockTracking_Gold.Ap.Model;

namespace StockTracking_Gold.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //get pbr test
            //Pbr Get_PbrFlow(string STOCK_ID, string CHT_CAT)
            string stock_id = "2330";
            string cht_cat = "MONTH";

            StockCrawler_WebClient client = new StockCrawler_WebClient(new System.Net.WebClient());
            var result = client.Get_PbrFlow(stock_id, cht_cat);
            Console.WriteLine(result);
            Console.ReadLine();

        }
        [TestMethod]
        public void TestMethod2()
        {
            //get pbr test
            //Pbr Get_PbrFlow(string STOCK_ID, string CHT_CAT)
            string stock_id = "2330";
            string cht_cat = "MONTH";

            StockCrawler client = new StockCrawler(new System.Net.WebClient());
            var result = client.Get_PbrFlow2(stock_id, cht_cat);
            Console.WriteLine(result);
            Console.ReadLine();

        }
    }
}

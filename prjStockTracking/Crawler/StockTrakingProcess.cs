using prjStockTracking.Models;
using prjStockTracking.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace prjStockTracking.Crawler
{
    internal class StockTrakingProcess
    {
        private StockCrawler stockCrawler;
        private List<StockInfo> stockInfos = new List<StockInfo>();

        // 建構子：new StockTrakingProcess()時會先執行
        // 建立StockTrakingProcess()物件時，同時建立StockCrawler()物件。
        public StockTrakingProcess()
        {
            stockCrawler = new StockCrawler(new System.Net.WebClient());
        }

        //public void StartStockCrawler
        public List<StockViewModel> StartStockCrawler(string market = "TSE", string onTime = "N", int gap = 4)
        {
            List<StockViewModel> dtoList = new List<StockViewModel>();
            try
            {
                // 1.取得黃金交叉個股清單 "TSE" or "OTC"
                //var goldList = stockCrawler.Get_GoldenCrossList(market).Where(o => o.Code.Length == 4).Take(3).ToList();
                var goldList = stockCrawler.Get_GoldenCrossList(market).Where(o => o.Code.Length == 4).ToList();

                // 2.1. onTime == "Y" // 即時更新才抓河流圖的本淨比&本益比。
                if (onTime == "Y")
                {
                    // 取得清單的本淨比及本益比河流圖數據階段
                    foreach (var gold in goldList)
                    {
                        int ranNum; //讓每次抓取間隔隨機
                        string stockID = gold.Code;
                        string date = "MONTH";

                        Pbr pbr_data = stockCrawler.Get_PbrFlow(stockID, date);
                        Thread.Sleep(gap * 1000);
                        ranNum = new Random().Next(gap-1, gap + 1);
                        Per per_data = stockCrawler.Get_PerFlow(stockID, date);
                        ranNum = new Random().Next(gap-1, gap + 1);
                        Thread.Sleep(gap * 1000);

                        stockInfos.Add(new StockInfo { Gold = gold, Pbr = pbr_data, Per = per_data });
                    }
                    // 填入要回傳的資料ViewModel
                    foreach (var item in stockInfos)
                    {
                        //黃金交叉+本益比本淨比河流
                        dtoList.Add(AutoMapperConfig.Mapper.Map<StockViewModel>(item));

                        //dtoList.Add(new StockViewModel
                        //{
                        //    //1.黃金交叉
                        //    Date = item.Gold.Date,
                        //    Market = item.Gold.Market,
                        //    Code = item.Gold.Code,
                        //    Name = item.Gold.Name,
                        //    Price = item.Gold.Price,
                        //    MA5 = item.Gold.MA5,
                        //    MA20 = item.Gold.MA20,
                        //    MA60 = item.Gold.MA60,
                        //    MA120 = item.Gold.MA120,
                        //    Indicators = item.Gold.Indicators,
                        //    //2.本益比本淨比河流
                        //    //pbr
                        //    FlowDate = item.Pbr.Date,
                        //    Transaction = item.Pbr.Transaction,
                        //    Volume = item.Pbr.Volume,
                        //    Total_amount = item.Pbr.Total_amount,
                        //    Url = item.Pbr.url,

                        //    Bps = item.Pbr.Price,
                        //    PbrLevel = item.Pbr.Flow_Level,
                        //    //per
                        //    Eps = item.Per.Eps_price,
                        //    PerLevel = item.Per.Flow_Level
                        //});
                    }
                }
                // 2.2. onTime == "N" // 不抓河流圖，直接將Gold填入要回傳的資料ViewModel。
                else
                {
                    foreach (var item in goldList)
                    {
                        //黃金交叉
                        dtoList.Add(AutoMapperConfig.Mapper.Map<StockViewModel>(item));

                        //dtoList.Add(new StockViewModel
                        //{
                        //    //黃金交叉
                        //    Date = item.Date,
                        //    Market = item.Market,
                        //    Code = item.Code,
                        //    Name = item.Name,
                        //    Price = item.Price,
                        //    MA5 = item.MA5,
                        //    MA20 = item.MA20,
                        //    MA60 = item.MA60,
                        //    MA120 = item.MA120,
                        //    Indicators = item.Indicators
                        //});
                    }
                }
                // 3.關閉瀏覽器
                stockCrawler.Dispose();
                // 4.輸出結果
                //Console.Write(JsonConvert.SerializeObject(dtoList, Formatting.Indented));
                return dtoList;
            }
            catch (Exception ex)
            {
                // 關閉瀏覽器
                stockCrawler.Dispose();
                // 輸出結果s
                //Console.Write(JsonConvert.SerializeObject(dtoList, Formatting.Indented));
                return dtoList;
            }
        }
    }
}
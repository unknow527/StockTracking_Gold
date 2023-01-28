using Newtonsoft.Json;
using StockTracking_Gold.Ap.Dto;
using StockTracking_Gold.Ap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace StockTracking_Gold.Ap.Applibs
{
    internal class StockTrakingProcess
    {
        private IStockCrawler stockCrawler;
        private List<StockInfo> stockInfos = new List<StockInfo>();

        // 建構子：new StockTrakingProcess()時會先執行
        // 建立StockTrakingProcess()物件時，同時建立StockCrawler()物件。
        public StockTrakingProcess()
        {
            //stockCrawler = new StockCrawler(new System.Net.WebClient());

            // 1. 使用Selenium
            stockCrawler = new StockCrawler_Selenium();
            // 2. 使用WebClient (目前在pbr per解析會有異常，待修復)
            //stockCrawler = new StockCrawler_WebClient(new System.Net.WebClient());
        }

        //public void StartStockCrawler
        public List<StockViewModelDtoV2> StartStockCrawler(string market = "TSE", string onTime = "N", int gap = 4)
        {
            TimeSpan timeSpan;
            DateTime start_time = DateTime.Now;
            List<StockViewModelDtoV2> dtoList = new List<StockViewModelDtoV2>();

            try
            {
                // 1.取得黃金交叉個股清單 "TSE" or "OTC"
                var goldList = stockCrawler.Get_GoldenCrossList(market).Where(o => o.Code.Length == 4).Take(3).ToList();
                //var goldList = stockCrawler.Get_GoldenCrossList(market).Where(o => o.Code.Length == 4).ToList();
                //var estimatedTime = (StockList.Count() / 2.5).ToString("0.00"); //預估時間
            
                // 2.1. onTime == "Y" // 即時更新才抓河流圖的本淨比&本益比。
                if (onTime == "Y")
                {
                    // 取得清單的本淨比及本益比河流圖數據階段
                    foreach (var gold in goldList)
                    {
                        string stockID = gold.Code;
                        string date = "MONTH";
                        Pbr pbr_data = stockCrawler.Get_PbrFlow(stockID, date);
                        Thread.Sleep(gap * 1000);
                        Per per_data = stockCrawler.Get_PerFlow(stockID, date);
                        Thread.Sleep(gap * 1000);
                        
                        stockInfos.Add(new StockInfo { Gold = gold, Pbr = pbr_data, Per = per_data });
                    }
                    //Console.Write(JsonConvert.SerializeObject(stockInfos, Formatting.Indented));
                    //Console.WriteLine("------------------------------------------------");

                    // 1. foreach填入要回傳的資料ViewModel (use automapper)
                    foreach (var item in stockInfos)
                    {
                        dtoList.Add(AutoMapperConfig.Mapper.Map<StockViewModelDtoV2>(item));
                    }
                    // 2. linq寫法
                    //dtoList = stockInfos.Select(o => AutoMapperConfig.Mapper.Map<StockViewModelDtoV2>(o)).ToList();

                }
                // 2.2. onTime == "N" // 不抓河流圖，直接將Gold填入要回傳的資料ViewModel。
                else
                {
                    foreach (var item in goldList)
                    {
                        dtoList.Add(AutoMapperConfig.Mapper.Map<StockViewModelDtoV2>(item));
                    }
                }

                timeSpan = DateTime.Now - start_time;
                var costTime = timeSpan.TotalMinutes.ToString("0.00"); //花費時間
             
                // 3.關閉瀏覽器
                stockCrawler.Dispose();
                // 4.輸出結果
                //Console.Write(JsonConvert.SerializeObject(dtoList, Formatting.Indented));
                return dtoList;
            }
            catch (Exception ex)
            {
                //ViewBag.errorMsg = ex.Message; //異常LOG MSG
                timeSpan = DateTime.Now - start_time;
                //costTime = timeSpan.TotalMinutes.ToString("0.00"); //花費時間
                //return View(StockList);

                // 關閉瀏覽器
                stockCrawler.Dispose();
                return dtoList;
            }
        }
    }
}

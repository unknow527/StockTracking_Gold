﻿using Newtonsoft.Json;
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
        private StockCrawler stockCrawler;
        private List<StockInfo> stockInfos = new List<StockInfo>();

        // 建構子：new StockTrakingProcess()時會先執行
        // 建立StockTrakingProcess()物件時，同時建立StockCrawler()物件。
        public StockTrakingProcess()
        {
            stockCrawler = new StockCrawler(new System.Net.WebClient());
        }

        //public void StartStockCrawler
        public List<StockViewModelDto> StartStockCrawler(string market = "TSE", string onTime = "N", int gap = 2)
        {
            TimeSpan timeSpan;
            DateTime start_time = DateTime.Now;
            List<StockViewModelDto> dtoList = new List<StockViewModelDto>();

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

                    // 填入要回傳的資料ViewModel
                    foreach (var item in stockInfos)
                    {
                        dtoList.Add(new StockViewModelDto
                        {
                            黃金交叉Dto = new 黃金交叉Dto
                            {
                                Date = item.Gold.Date,
                                Market = item.Gold.Market,
                                Code = item.Gold.Code,
                                Name = item.Gold.Name,
                                Price = item.Gold.Price,
                                MA5 = item.Gold.MA5,
                                MA20 = item.Gold.MA20,
                                MA60 = item.Gold.MA60,
                                MA120 = item.Gold.MA120,
                                Indicators = item.Gold.Indicators
                            },
                            本益比本淨比河流Dto = new 本益比本淨比河流Dto
                            {
                                //pbr
                                FlowDate = item.Pbr.Date,
                                Transaction = item.Pbr.Transaction,
                                Volume = item.Pbr.Volume,
                                Total_amount = item.Pbr.Total_amount,
                                Url = item.Pbr.url,

                                Bps = item.Pbr.Price,
                                PbrLevel = item.Pbr.Flow_Level,
                                //per
                                Eps = item.Per.Eps_price,
                                PerLevel = item.Per.Flow_Level
                            }
                        });
                    }
                    /**
                    // 填入要回傳的資料ViewModel(寫法2 linq)，同foreach用法 
                    List<StockViewModelDto> dtoList = stockInfos.Select(o => new StockViewModelDto
                    {
                        黃金交叉Dto = new 黃金交叉Dto
                        {
                            Code = o.Gold.Code,
                            Date = o.Gold.Date,
                            Indicators = o.Gold.Indicators,
                            MA120 = o.Gold.Indicators
                        },
                        本益比本淨比河流Dto = new 本益比本淨比河流Dto
                        {
                            Eps = o.Per.Eps_price,
                            PerLevel = o.Per.Flow_Level
                        }
                    }).ToList();
                    //List<StockViewModelDto> dtoList = new List<StockViewModelDto>();
                    **/
                }
                // 2.2. onTime == "N" // 不抓河流圖，直接將Gold填入要回傳的資料ViewModel。
                else
                {
                    foreach (var item in goldList)
                    {
                        dtoList.Add(new StockViewModelDto
                        {
                            黃金交叉Dto = new 黃金交叉Dto
                            {
                                Date = item.Date,
                                Market = item.Market,
                                Code = item.Code,
                                Name = item.Name,
                                Price = item.Price,
                                MA5 = item.MA5,
                                MA20 = item.MA20,
                                MA60 = item.MA60,
                                MA120 = item.MA120,
                                Indicators = item.Indicators
                            },
                        });
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
                // 輸出結果
                //Console.Write(JsonConvert.SerializeObject(dtoList, Formatting.Indented));
                return dtoList;
            }
        }
    }
}

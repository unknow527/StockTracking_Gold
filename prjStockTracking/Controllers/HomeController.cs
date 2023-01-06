using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjStockTracking.Models;
using prjStockTracking.Crawler;
using System.Threading;

namespace prjStockTracking.Controllers
{
    public class HomeController : Controller
    {

        List<StockViewModel> StockList = new List<StockViewModel>();
        string date = DateTime.Today.ToString();

        public ActionResult Index(string market = "TSE", string onTime = "N", int gap = 2)
        {
            TimeSpan timeSpan;
            DateTime start_time = DateTime.Now;
            try
            {
                // 1.取得黃金交叉個股清單 //TSE OTC
                StockList = Crawler.StockCrawler.Get_GoldenCrossList(market);
                ViewBag.estimatedTime = (StockList.Count() / 2.5).ToString("0.00"); //預估時間
                // 2.即時更新才抓河流圖 //本淨比 本益比
                if (onTime == "Y")
                {
                    // 取得清單的本淨比及本益比河流圖數據階段
                    foreach (var i in StockList)
                    {
                        // 代碼為4碼才執行
                        if (i.Code.Length == 4)
                        {
                            string stockID = i.Code;
                            string date = "MONTH";
                            // 抓GoodInfo本淨比資料
                            Thread.Sleep(gap*1000);
                            PbrViewModel pbr_data = Crawler.StockCrawler.Get_PbrFlow(stockID, date);
                            i.Url = pbr_data.url; //GoodInfo連結
                            i.PbrLevel = pbr_data.Flow_Level; //本淨比河流LEVEL區間
                            i.Volume = pbr_data.Volume; //成交量
                            i.Total_amount = pbr_data.Total_amount; //成交金額
                            i.Transaction = pbr_data.Transaction;
                            i.FlowDate = pbr_data.Date;
                            i.Bps = pbr_data.Bps_price;
                            // 抓GoodInfo本益比資料
                            Thread.Sleep(gap*1000);
                            PerViewModel per_data = Crawler.StockCrawler.Get_PerFlow(stockID, date);
                            i.PerLevel = per_data.Flow_Level; //本益比河流LEVEL區間
                            i.Eps = per_data.Eps_price;
                        }
                    }
                }
                timeSpan = DateTime.Now - start_time;
                ViewBag.costTime = timeSpan.TotalMinutes.ToString("0.00"); //花費時間
                return View(StockList);
            }
            catch (Exception ex)
            {
                ViewBag.errorMsg = ex.Message; //異常LOG MSG
                timeSpan = DateTime.Now - start_time;
                ViewBag.costTime = timeSpan.TotalMinutes.ToString("0.00"); //花費時間
                return View(StockList); 
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

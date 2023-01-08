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

        //List<StockViewModel> StockList = new List<StockViewModel>();

        public ActionResult Index(string market = "TSE", string onTime = "N", int gap = 4)
        {
            TimeSpan timeSpan;
            DateTime start_time = DateTime.Now;
            StockTrakingProcess process = new StockTrakingProcess();
            var StockList = process.StartStockCrawler(market, onTime, gap);
            try
            {
                //取資料
                StockList = process.StartStockCrawler(market, onTime, gap);
                //花費時間
                timeSpan = DateTime.Now - start_time;
                ViewBag.costTime = timeSpan.TotalMinutes.ToString("0.00");
                //預估時間
                double ct = StockList.Count();
                ViewBag.estimatedTime = Math.Round((ct*(2 + gap*2)/60)+1, 2);
                return View(StockList);
            }
            catch (Exception ex)
            {
                //異常LOG MSG
                ViewBag.errorMsg = ex.Message;
                //花費時間
                timeSpan = DateTime.Now - start_time;
                ViewBag.costTime = timeSpan.TotalMinutes.ToString("0.00");
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

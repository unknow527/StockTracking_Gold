using StockTracking_Gold.Ap.Applibs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StockTracking_Gold.Ap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var t1 = Task.Run(() =>
            {
                StockTrakingProcess process = new StockTrakingProcess();
                process.StartStockCrawler("TSE", "N", 8);
            });
            Thread.Sleep(4000);
            var t2 = Task.Run(() =>
            {
                StockTrakingProcess process = new StockTrakingProcess();
                process.StartStockCrawler("OTC", "N", 8);
            });

            Task.WaitAll(t1, t2);

            Console.ReadLine();
        }
    }
}


/*
 
Program 程式進入點 
 0. 建立主程式 new xxxProcess()
 1. 透過主程式去執行  xxxProcess.Start()
 2. 期望主程式持續地持行 N分鐘去抓取資料一次 

* 讓關注的事情愈少愈好
* 主程式
*   制定定期工作
*   定期工作 - 產生參數給爬蟲抓資料
* 
* 爬蟲
*   根據參數去抓取資料
*   紀錄每筆抓取花費的時間
*   
* error捕獲
* 


*/
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using prjStockTracking.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace prjStockTracking.Crawler
{
    public class StockCrawler
    {
        private WebClient _client;
        private HtmlDocument doc = new HtmlDocument();
        private MemoryStream ms = new MemoryStream();
        private IWebDriver driver;

        //非靜態方法，使用需先new()出物件，會啟動建構子
        public StockCrawler(WebClient client)
        {
            driver = new ChromeDriver();
            _client = client;
            _client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36)");
        }

        // Selenium方法
        //取得鉅亨網--黃金交叉的個股清單 //Selenium+HtmlDocument
        public List<Gold> Get_GoldenCrossList(string market) //**參數: 市場別(TSE/OTC)
        {
            List<Gold> goldList = new List<Gold>();

            //從鉅亨網查黃金交叉個股清單
            string url = "https://www.cnyes.com/twstock/a_technical7.aspx";

            //操作瀏覽器篩選功能
            driver.Navigate().GoToUrl(url);

            Thread.Sleep(1000);
            var select = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_D1")); //下拉選單
            var selectElement = new SelectElement(select);
            selectElement.SelectByValue(market); // market = "TSE" or "OTC"
            //selectElement.SelectByText("集中市場");
            Thread.Sleep(1000);//等待資料刷新
            string html = driver.PageSource;

            //處理資料
            //HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            string xpath;
            for (int tb_count = 1; tb_count < 6; tb_count++)
            {
                xpath = "//*[@id=\"ctl00_ContentPlaceHolder1_UpdatePanel1\"]/div[1]/div/table[" + tb_count + "]";
                int tr_count = 1;
                string indicators = "";
                HtmlNodeCollection tb_var = doc.DocumentNode.SelectNodes(xpath);

                if (tb_var != null)
                {
                    HtmlNodeCollection tr_var = null;

                    do
                    {
                        //Xpath：透過 Webclient取得的要去掉tbody，但 IWebDriver.PageSource則要保留tbody
                        string tr_xpath = xpath + "/tbody/tr[" + tr_count + "]";
                        tr_var = doc.DocumentNode.SelectNodes(tr_xpath);

                        if (tr_var == null)
                        {
                            //空值不動作，結束迴圈 
                        }
                        else if (tr_count == 1)
                        {
                            //取得table名稱 (技術指標)
                            indicators = doc.DocumentNode.SelectNodes(tr_xpath + "/th")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            tr_count++;
                        }
                        else if (tr_count > 2)
                        {
                            //取得股票清單資料
                            string code = doc.DocumentNode.SelectNodes(tr_xpath + "/td[1]/a")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            string name = doc.DocumentNode.SelectNodes(tr_xpath + "/td[2]/a")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            string price = doc.DocumentNode.SelectNodes(tr_xpath + "/td[3]")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            string ma5 = doc.DocumentNode.SelectNodes(tr_xpath + "/td[4]")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            string ma20 = doc.DocumentNode.SelectNodes(tr_xpath + "/td[5]")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            string ma60 = doc.DocumentNode.SelectNodes(tr_xpath + "/td[6]")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            string ma120 = doc.DocumentNode.SelectNodes(tr_xpath + "/td[7]")[0].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "").ToString();
                            string x = "//*[@id=" + "'ctl00_ContentPlaceHolder1_D3'" + "]/option[1]";
                            string date = doc.DocumentNode.SelectNodes(x)[0].InnerText.ToString();
                            goldList.Add(
                                new Gold() { Code = code, Name = name, Price = price, MA5 = ma5, MA20 = ma20, MA60 = ma60, MA120 = ma120, Indicators = indicators, Market = market, Date = date }
                            );
                            tr_count++;
                        }
                        else
                        {
                            //EX. (tr_count == 2) =>> tr[2] 為欄位名稱資料，不動作，繼續下個迴圈。
                            tr_count++;
                        }

                    } while (tr_var != null); //tr找不到資料時停止
                }
                else
                {
                    string code = "error";
                    string name = xpath;
                    goldList.Add(new Gold() { Code = code, Name = name });
                }
            }

            return goldList;
        }
        //取得GoodInfo--個股本淨比河流  //Selenium+HtmlDocument
        public Pbr Get_PbrFlow(string STOCK_ID, string CHT_CAT)
        {
            string url = "https://goodinfo.tw/tw/";
            string new_url = url + "ShowK_ChartFlow.asp?RPT_CAT=PBR&STOCK_ID=" + STOCK_ID + "&CHT_CAT=" + CHT_CAT;

            //操作瀏覽器篩選功能
            driver.Navigate().GoToUrl(new_url);
            Thread.Sleep(2000);
            string html = driver.PageSource;

            //處理資料
            //HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // 目標網站的XPath、Webclient方法時要去除瀏覽器文本會自動產生tbody
            string Xpath = "/html/body/table[2]/tbody/tr/td[3]/div/div/div/table/tbody/tr[3]";
            //string nXpath = Xpath.Replace("/tbody", "");
            string nXpath = Xpath;
            //Thread.Sleep(500);

            // 取得資料
            // 表頭
            string id = STOCK_ID;
            string cht_cat = CHT_CAT;
            string date = doc.DocumentNode.SelectSingleNode("/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[1]/th/table/tbody/tr/td[last()]/nobr").InnerText.ToString();
            string volume = doc.DocumentNode.SelectSingleNode("/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[1]").InnerText.ToString();
            string total_amount = doc.DocumentNode.SelectSingleNode("/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[2]/nobr").InnerText.Replace("&nbsp;", "").ToString();
            // Table         
            string transaction = doc.DocumentNode.SelectSingleNode(nXpath + "/td[1]").InnerText.ToString();
            string price = doc.DocumentNode.SelectSingleNode(nXpath + "/td[2]").InnerText.ToString();
            string updown = doc.DocumentNode.SelectSingleNode(nXpath + "/td[3]").InnerText.ToString();
            string updown_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[4]").InnerText.ToString();
            string bps = doc.DocumentNode.SelectSingleNode(nXpath + "/td[5]").InnerText.ToString();
            string pbr_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[6]").InnerText.ToString();
            string l1 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[7]").InnerText.ToString();
            string l2 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[8]").InnerText.ToString();
            string l3 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[9]").InnerText.ToString();
            string l4 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[10]").InnerText.ToString();
            string l5 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[11]").InnerText.ToString();
            string l6 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[12]").InnerText.ToString();
            string pbr_url = new_url;
            string level = "";
            //執行比對，判斷目前股價所在LEVEL區間
            //有資料、且股票代碼為四碼才執行比對
            if (bps != "" & STOCK_ID.Length == 4)
            {
                // BPS大於0才比對
                if (Convert.ToDouble(bps) > 0)
                {
                    //判斷股價區間
                    if (Convert.ToDouble(price) < Convert.ToDouble(l1))
                    {
                        level = "L1:極低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l2))
                    {
                        level = "L2:很低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l3))
                    {
                        level = "L3:低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l4))
                    {
                        level = "L4:中";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l5))
                    {
                        level = "L5:高";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l6))
                    {
                        level = "L6:很高";
                    }
                    else if (Convert.ToDouble(price) > Convert.ToDouble(l6))
                    {
                        level = "L7:極高";
                    }
                    else
                    {
                        level = "";
                    }
                }
            }
            Pbr list = new Pbr() { Id = id, Cht_cat = cht_cat, Date = date, Transaction = transaction, Price = price, Volume = volume, Total_amount = total_amount, Updown = updown, Updown_percent = updown_percent, Bps_price = bps, Pbr_percent = pbr_percent, Flow_L1 = l1, Flow_L2 = l2, Flow_L3 = l3, Flow_L4 = l4, Flow_L5 = l5, Flow_L6 = l6, url = pbr_url, Flow_Level = level };
            return list;
        }
        //取得GoodInfo--個股本益比河流  //Selenium+HtmlDocument
        public Per Get_PerFlow(string STOCK_ID, string CHT_CAT)
        {
            //if (STOCK_ID.Length != 4)
            //{
            //    return null;
            //}
            string url = "https://goodinfo.tw/tw/";
            string new_url = url + "ShowK_ChartFlow.asp?RPT_CAT=PER&STOCK_ID=" + STOCK_ID + "&CHT_CAT=" + CHT_CAT;

            //操作瀏覽器篩選功能
            driver.Navigate().GoToUrl(new_url);
            Thread.Sleep(2000);
            string html = driver.PageSource;


            //處理資料
            //HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // 目標網站的XPath、去除瀏覽器文本會自動產生tbody
            string Xpath = "/html/body/table[2]/tbody/tr/td[3]/div/div/div/table/tbody/tr[3]";
            //string nXpath = Xpath.Replace("/tbody", "");
            string nXpath = Xpath;
            //Thread.Sleep(500);

            // 取得資料
            // 表頭
            string id = STOCK_ID;
            string cht_cat = CHT_CAT;

            string date = doc.DocumentNode.SelectSingleNode("/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[1]/th/table/tbody/tr/td[last()]/nobr").InnerText.ToString();
            string volume = doc.DocumentNode.SelectSingleNode("/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[1]").InnerText.ToString();
            string total_amount = doc.DocumentNode.SelectSingleNode("/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[2]/nobr").InnerText.Replace("&nbsp;", "").ToString();
            // table
            string transaction = doc.DocumentNode.SelectSingleNode(nXpath + "/td[1]").InnerText.ToString();
            string price = doc.DocumentNode.SelectSingleNode(nXpath + "/td[2]").InnerText.ToString();
            string updown = doc.DocumentNode.SelectSingleNode(nXpath + "/td[3]").InnerText.ToString();
            string updown_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[4]").InnerText.ToString();
            string eps = doc.DocumentNode.SelectSingleNode(nXpath + "/td[5]").InnerText.ToString();
            string per_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[6]").InnerText.ToString();
            string l1 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[7]").InnerText.ToString();
            string l2 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[8]").InnerText.ToString();
            string l3 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[9]").InnerText.ToString();
            string l4 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[10]").InnerText.ToString();
            string l5 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[11]").InnerText.ToString();
            string l6 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[12]").InnerText.ToString();
            string per_url = new_url;
            string level = "";
            if (eps != "" & STOCK_ID.Length == 4)
            {
                // EPS大於0才比對
                if (Convert.ToDouble(eps) > 0)
                {
                    //判斷股價區間
                    if (Convert.ToDouble(price) < Convert.ToDouble(l1))
                    {
                        level = "L1:極低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l2))
                    {
                        level = "L2:很低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l3))
                    {
                        level = "L3:低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l4))
                    {
                        level = "L4:中";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l5))
                    {
                        level = "L5:高";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l6))
                    {
                        level = "L6:很高";
                    }
                    else if (Convert.ToDouble(price) > Convert.ToDouble(l6))
                    {
                        level = "L7:極高";
                    }
                    else
                    {
                        level = "";
                    }
                }


            }
            Per list = new Per() { Id = id, Cht_cat = cht_cat, Date = date, Transaction = transaction, Price = price, Volume = volume, Total_amount = total_amount, Updown = updown, Updown_percent = updown_percent, Eps_price = eps, Per_percent = per_percent, Flow_L1 = l1, Flow_L2 = l2, Flow_L3 = l3, Flow_L4 = l4, Flow_L5 = l5, Flow_L6 = l6, url = per_url, Flow_Level = level };

            return list;
        }


        // WebClient方法
        //取得GoodInfo--個股本淨比河流數據  //webclient + HtmlDocument
        public Pbr Get_PbrFlow2(string STOCK_ID, string CHT_CAT)
        {
            if (STOCK_ID.Length != 4)
            {
                return null;
            }
            string url = "https://goodinfo.tw/tw/";
            string new_url = url + "ShowK_ChartFlow.asp?RPT_CAT=PBR&STOCK_ID=" + STOCK_ID + "&CHT_CAT=" + CHT_CAT;
            //Console.WriteLine(new_url);
            //將網頁來源資料暫存到記憶體內
            ms = new MemoryStream(_client.DownloadData(new_url));

            //HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.UTF8); // 使用 UTF8 編碼讀入 HTML

            // 檢查資料，將stream轉字串
            //string str = System.Text.Encoding.Default.GetString(ms.ToArray());
            //Console.WriteLine(str);

            // 目標網站的XPath、WebClient方法要去除瀏覽器文本會自動產生tbody
            string Xpath = "/html/body/table[2]/tbody/tr/td[3]/div/div/div/table/tbody/tr[3]";
            string nXpath = Xpath.Replace("/tbody", "");

            // 取得資料----
            string id = STOCK_ID;
            string cht_cat = CHT_CAT;

            // 表頭資料
            string date = "/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[1]/th/table/tbody/tr/td[last()]/nobr";
            date = doc.DocumentNode.SelectSingleNode(date.Replace("/tbody", "")).InnerText.ToString();

            string volume = "/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[1]";
            volume = doc.DocumentNode.SelectSingleNode(volume.Replace("/tbody", "")).InnerText.ToString();

            string total_amount = "/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[2]/nobr";
            total_amount = doc.DocumentNode.SelectSingleNode(total_amount.Replace("/tbody", "")).InnerText.Replace("&nbsp;", "").ToString();

            // table資料
            string transaction = doc.DocumentNode.SelectSingleNode(nXpath + "/td[1]").InnerText.ToString();
            string price = doc.DocumentNode.SelectSingleNode(nXpath + "/td[2]").InnerText.ToString();
            string updown = doc.DocumentNode.SelectSingleNode(nXpath + "/td[3]").InnerText.ToString();
            string updown_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[4]").InnerText.ToString();
            string bps = doc.DocumentNode.SelectSingleNode(nXpath + "/td[5]").InnerText.ToString();
            string pbr_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[6]").InnerText.ToString();
            string l1 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[7]").InnerText.ToString();
            string l2 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[8]").InnerText.ToString();
            string l3 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[9]").InnerText.ToString();
            string l4 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[10]").InnerText.ToString();
            string l5 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[11]").InnerText.ToString();
            string l6 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[12]").InnerText.ToString();
            string pbr_url = new_url;

            //河流圖區間
            string level = "";
            //執行比對，判斷目前股價所在LEVEL區間
            //有資料、且股票代碼為四碼才執行比對
            if (bps != "" & STOCK_ID.Length == 4)
            {
                // BPS大於0才比對
                if (Convert.ToDouble(bps) > 0)
                {
                    //判斷股價區間
                    if (Convert.ToDouble(price) < Convert.ToDouble(l1))
                    {
                        level = "L1:極低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l2))
                    {
                        level = "L2:很低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l3))
                    {
                        level = "L3:低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l4))
                    {
                        level = "L4:中";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l5))
                    {
                        level = "L5:高";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l6))
                    {
                        level = "L6:很高";
                    }
                    else if (Convert.ToDouble(price) > Convert.ToDouble(l6))
                    {
                        level = "L7:極高";
                    }
                    else
                    {
                        level = "";
                    }
                }
            }
            Pbr list = new Pbr() { Id = id, Cht_cat = cht_cat, Date = date, Transaction = transaction, Price = price, Volume = volume, Total_amount = total_amount, Updown = updown, Updown_percent = updown_percent, Bps_price = bps, Pbr_percent = pbr_percent, Flow_L1 = l1, Flow_L2 = l2, Flow_L3 = l3, Flow_L4 = l4, Flow_L5 = l5, Flow_L6 = l6, url = pbr_url, Flow_Level = level };
            return list;
        }
        //取得GoodInfo--個股本益比河流  //webclient + HtmlDocument
        public Per Get_PerFlow2(string STOCK_ID, string CHT_CAT)
        {
            if (STOCK_ID.Length != 4)
            {
                return null;
            }
            string url = "https://goodinfo.tw/tw/";
            string new_url = url + "ShowK_ChartFlow.asp?RPT_CAT=PBR&STOCK_ID=" + STOCK_ID + "&CHT_CAT=" + CHT_CAT;
            //Console.WriteLine(new_url);
            //將網頁來源資料暫存到記憶體內
            ms = new MemoryStream(_client.DownloadData(new_url));

            //HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.UTF8); // 使用 UTF8 編碼讀入 HTML

            // 檢查資料，將stream轉字串
            //string str = System.Text.Encoding.Default.GetString(ms.ToArray());
            //Console.WriteLine(str);

            // 目標網站的XPath、WebClient方法要去除瀏覽器文本會自動產生tbody
            string Xpath = "/html/body/table[2]/tbody/tr/td[3]/div/div/div/table/tbody/tr[3]";
            string nXpath = Xpath.Replace("/tbody", "");
            Thread.Sleep(500);

            // 取得資料
            // 表頭
            string id = STOCK_ID;
            string cht_cat = CHT_CAT;

            string date = "/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[1]/th/table/tbody/tr/td[last()]/nobr";
            date = doc.DocumentNode.SelectSingleNode(date.Replace("/tbody", "")).InnerText.ToString();

            string volume = "/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[1]";
            volume = doc.DocumentNode.SelectSingleNode(volume.Replace("/tbody", "")).InnerText.ToString();

            string total_amount = "/html/body/table[2]/tbody/tr/td[3]/table[1]/tbody/tr/td[1]/table/tbody/tr[5]/td[2]/nobr";
            total_amount = doc.DocumentNode.SelectSingleNode(total_amount.Replace("/tbody", "")).InnerText.Replace("&nbsp;", "").ToString();

            // table
            string transaction = doc.DocumentNode.SelectSingleNode(nXpath + "/td[1]").InnerText.ToString();
            string price = doc.DocumentNode.SelectSingleNode(nXpath + "/td[2]").InnerText.ToString();
            string updown = doc.DocumentNode.SelectSingleNode(nXpath + "/td[3]").InnerText.ToString();
            string updown_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[4]").InnerText.ToString();
            string eps = doc.DocumentNode.SelectSingleNode(nXpath + "/td[5]").InnerText.ToString();
            string per_percent = doc.DocumentNode.SelectSingleNode(nXpath + "/td[6]").InnerText.ToString();
            string l1 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[7]").InnerText.ToString();
            string l2 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[8]").InnerText.ToString();
            string l3 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[9]").InnerText.ToString();
            string l4 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[10]").InnerText.ToString();
            string l5 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[11]").InnerText.ToString();
            string l6 = doc.DocumentNode.SelectSingleNode(nXpath + "/td[12]").InnerText.ToString();
            string per_url = new_url;

            //河流圖區間
            string level = "";
            //執行比對，判斷目前股價所在LEVEL區間
            //有資料、且股票代碼為四碼才執行比對
            if (eps != "" & STOCK_ID.Length == 4)
            {
                // BPS大於0才比對
                if (Convert.ToDouble(eps) > 0)
                {
                    //判斷股價區間
                    if (Convert.ToDouble(price) < Convert.ToDouble(l1))
                    {
                        level = "L1:極低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l2))
                    {
                        level = "L2:很低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l3))
                    {
                        level = "L3:低";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l4))
                    {
                        level = "L4:中";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l5))
                    {
                        level = "L5:高";
                    }
                    else if (Convert.ToDouble(price) < Convert.ToDouble(l6))
                    {
                        level = "L6:很高";
                    }
                    else if (Convert.ToDouble(price) > Convert.ToDouble(l6))
                    {
                        level = "L7:極高";
                    }
                    else
                    {
                        level = "";
                    }
                }
            }
            Per list = new Per() { Id = id, Cht_cat = cht_cat, Date = date, Transaction = transaction, Price = price, Volume = volume, Total_amount = total_amount, Updown = updown, Updown_percent = updown_percent, Eps_price = eps, Per_percent = per_percent, Flow_L1 = l1, Flow_L2 = l2, Flow_L3 = l3, Flow_L4 = l4, Flow_L5 = l5, Flow_L6 = l6, url = per_url, Flow_Level = level };
            return list;
        }

        // 關閉瀏覽器
        public void Dispose()
        {
            driver.Close();
        }
    }
}

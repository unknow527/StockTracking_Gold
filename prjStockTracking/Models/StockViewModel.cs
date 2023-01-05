using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjStockTracking.Models
{
    public class StockViewModel
    {
        //黃金交叉
        public string Code { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string MA5 { get; set; }
        public string MA20 { get; set; }
        public string MA60 { get; set; }
        public string MA120 { get; set; }
        public string Indicators { get; set; } //指標
        public string Market { get; set; } //市場別
        public string Date { get; set; }

        //本益比本淨比河流
        public string FlowDate { get; set; }
        public string Transaction { get; set; }
        public string Volume { get; set; }
        public string Total_amount { get; set; }
        public string Bps { get; set; }
        public string Eps { get; set; }
        public string PbrLevel { get; set; }
        public string PerLevel { get; set; }
        public string Url { get; set; }
    }
}
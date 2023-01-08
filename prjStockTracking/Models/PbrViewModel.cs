using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjStockTracking.Models
{
    public class PbrViewModel
    {
        // 股價淨值比PBR / 每股淨值BPS
        // id, cht_cat, date, price, updown, updown_percent, bps, bps_percent, pb_l1, pb_l2, pb_l3, pb_l4, pb_l5, pb_l6
        public string Date { get; set; } //資料日期
        public string Id { get; set; }
        public string Cht_cat { get; set; }
        public string Transaction { get; set; } //交易期間
        public string Price { get; set; }
        public string Volume { get; set; }
        public string Total_amount { get; set; }
        public string Updown { get; set; }
        public string Updown_percent { get; set; }
        public string Bps_price { get; set; }
        public string Pbr_percent { get; set; } 
        public string Flow_L1 { get; set; } 
        public string Flow_L2 { get; set; }
        public string Flow_L3 { get; set; }
        public string Flow_L4 { get; set; }
        public string Flow_L5 { get; set; }
        public string Flow_L6 { get; set; }
        public string url { get; set; }
        public string Flow_Level { get; set; }
    }
}
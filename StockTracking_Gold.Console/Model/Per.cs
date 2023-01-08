namespace StockTracking_Gold.Ap.Model
{
    public class Per
    {
        public string Date { get; set; } //資料日期
        public string Id { get; set; }
        public string Cht_cat { get; set; }
        public string Transaction { get; set; } //交易期間
        public string Price { get; set; }
        public string Volume { get; set; }
        public string Total_amount { get; set; }
        public string Updown { get; set; }
        public string Updown_percent { get; set; }
        public string Eps_price { get; set; }
        public string Per_percent { get; set; }
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

namespace prjStockTracking.Models
{
    public class Gold
    {
        public string Date { get; set; } //資料日期
        public string Market { get; set; } //市場別
        public string Code { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string MA5 { get; set; }
        public string MA20 { get; set; }
        public string MA60 { get; set; }
        public string MA120 { get; set; }
        public string Indicators { get; set; } //指標
    }
}
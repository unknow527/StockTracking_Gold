namespace StockTracking_Gold.Ap.Dto
{
    /// <summary>
    /// 丟給前端的強型別模型
    /// Data Transfor Object
    /// </summary>
    public class StockViewModelDto
    {
        public 黃金交叉Dto 黃金交叉Dto { get; set; }
        public 本益比本淨比河流Dto 本益比本淨比河流Dto { get; set; }
    }

    public class 黃金交叉Dto
    {
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
    }

    public class 本益比本淨比河流Dto
    {
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
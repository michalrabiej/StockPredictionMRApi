namespace StockPredictionMRApi.Models
{
    public class CryptoData
    {
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public float Volume { get; set; }
        public string Symbol { get; set; }
    }
}

namespace StockPredictionMRApi.Models
{
    internal class PredictionSeriesResponse
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public List<float> PredictedClosePrices { get; set; }
        public InputData InputData { get; set; }
    }
}
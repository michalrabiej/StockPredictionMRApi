namespace StockPredictionMRApi.Models
{
    internal class PredictionResponse
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public float PredictedClosePrice { get; set; }
        public object InputData { get; set; }
    }
}
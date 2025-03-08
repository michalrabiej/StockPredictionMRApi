namespace StockPredictionMRApi.Models;

public class CryptoHistoryDataViewModel
{
    public string Symbol { get; set; }
    public List<HistoryPrice> HistoryPrice { get; set; }
}
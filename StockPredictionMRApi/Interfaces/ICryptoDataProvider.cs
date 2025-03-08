using StockPredictionMRApi.Models;

namespace StockPredictionMRApi.Interfaces
{
    public interface ICryptoDataProvider
    {
        Task<List<CryptoDataEntity>> GetCryptoData(string symbol);
    }
}

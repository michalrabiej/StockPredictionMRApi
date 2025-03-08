using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockPredictionMRApi.DbContext;
using StockPredictionMRApi.Models;
using System.Data.Entity;

namespace StockPredictionMRApi.Services
{
    public class CryptoDataService
    {
        private readonly HttpClient _httpClient;
        private readonly StockDbContext _dbContext;
        private readonly IMapper _mapper;

        public CryptoDataService(HttpClient httpClient, StockDbContext dbContext, IMapper mapper)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task FetchAndSaveStockDataAsync(string symbol)
        {
            string cryptoCompareSymbol = symbol.ToUpper(); // CryptoCompare używa symboli w formacie np. BTC, ETH, XRP.

            var fromDate = new DateTime(2015, 1, 1).ToUniversalTime();
            var toDate = DateTime.UtcNow; // Dzisiejsza data
            var fromUnix = ((DateTimeOffset)fromDate).ToUnixTimeSeconds();

            // CryptoCompare dostarcza dane dzienne dla przeszłości
            string url = $"https://min-api.cryptocompare.com/data/v2/histoday?fsym={cryptoCompareSymbol}&tsym=USD&allData=true";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", $"Apikey {Config.ApiKey}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch data from CryptoCompare: {response.ReasonPhrase}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonData = JsonConvert.DeserializeObject<JObject>(jsonResponse);

            // Sprawdzenie struktury odpowiedzi
            if (jsonData?["Data"]?["Data"] is not JArray dataArray)
            {
                throw new Exception("Invalid response structure from CryptoCompare API.");
            }

            var newEntities = new List<CryptoDataEntity>();

            // Iteracja przez dane historyczne
            foreach (var item in dataArray)
            {
                var timestamp = (long)item["time"];
                var open = (float)item["open"];
                var high = (float)item["high"];
                var low = (float)item["low"];
                var close = (float)item["close"];
                var volume = (float)item["volumeto"];

                var date = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;

                var cryptoData = new CryptoDataEntity
                {
                    Date = date,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    Volume = volume,
                    Symbol = symbol
                };

                var cryptoDataEntity = _mapper.Map<CryptoDataEntity>(cryptoData);

                if (!_dbContext.CryptoData.Any(c => c.Symbol == cryptoDataEntity.Symbol && c.Date == cryptoDataEntity.Date))
                {
                    newEntities.Add(cryptoDataEntity);
                }
            }

            _dbContext.CryptoData.AddRange(newEntities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CryptoDataEntity>> GetLatestDataAsync(string symbol)
        {
            Console.WriteLine(_dbContext.CryptoData.GetType().FullName);

            var canConnect = await _dbContext.Database.CanConnectAsync();
            
            Console.WriteLine(canConnect ? "Connected to database" : "Failed to connect to database");

            var latestData = _dbContext.CryptoData
                .AsQueryable()
                .Where(c => c.Symbol == symbol)
                .OrderByDescending(c => c.Date)
                .ToArray();

            var latestDataModel = _mapper.Map<List<CryptoDataEntity>>(latestData);

            return latestDataModel;
        }
    }


}

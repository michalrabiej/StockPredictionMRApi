using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockPredictionMRApi.Models;
using StockPredictionMRApi.Services;

namespace StockPredictionMRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly CryptoDataService _cryptoService;
        private readonly IMapper _mapper;
        private readonly PredictionModel _predictionModel;

        public StockController(CryptoDataService cryptoService, IMapper mapper, PredictionModel predictionModel)
        {
            _cryptoService = cryptoService;
            _mapper = mapper;
            _predictionModel = predictionModel;
        }

        [HttpPost("{symbol}")]
        public async Task<IActionResult> FetchAndSaveCryptoData([FromRoute] string symbol)
        {
            try
            {
                await _cryptoService.FetchAndSaveStockDataAsync(symbol);
                return Ok("Data fetched and saved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("prediction/{symbol}")]
        public async Task<IActionResult> GetCryptoPrediction(string symbol)
        {
            try
            {
                // Pobierz dane z bazy dla podanego symbolu
                var latestData = await _cryptoService.GetLatestDataAsync(symbol);

                if (latestData == null || !latestData.Any())
                {
                    return BadRequest("No data available for the specified symbol.");
                }

                // Odwracamy kolejność, aby dane były chronologiczne
                latestData = latestData.OrderBy(c => c.Date).ToList();

                // Zainicjalizuj model na podstawie danych
                await _predictionModel.InitializeModelAsync(symbol);

                // Uzyskaj prognozy na podstawie ostatnich danych
                var predictedPrices = _predictionModel.PredictSeries(latestData, 7); // Prognozujemy na 3 dni do przodu

                // Tworzymy listę prognoz z datami
                var lastDate = latestData.Last().Date; // Pobieramy ostatnią datę z danych
                var predictedDataWithDates = predictedPrices
                    .Select((price, index) => new
                    {
                        Date = price.Date,
                        PredictedPrice = price.PredictedPrice
                    })
                    .ToList();

                return Ok(new
                {
                    Symbol = symbol,
                    Predictions = predictedDataWithDates // Zwracamy listę obiektów z datą i prognozą
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        
        [HttpGet("history/{symbol}")]
        public async Task<IActionResult> GetCryptoHistory([FromRoute] string symbol)
        {
            try
            {
                var historyData = await _cryptoService.GetLatestDataAsync(symbol);
                
                var pricesWithDates = historyData
                    .Select((price, index) => new HistoryPrice
                    {
                        Date = price.Date,
                        Price = price.Close
                    })
                    .ToList();

                var history = new CryptoHistoryDataViewModel
                {
                    Symbol = symbol,
                    HistoryPrice = pricesWithDates // Teraz to już poprawny typ List<PriceDateViewModel>
                };

                return Ok(history);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

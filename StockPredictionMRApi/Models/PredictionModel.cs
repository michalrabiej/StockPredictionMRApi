namespace StockPredictionMRApi.Models
{
    using Microsoft.ML;
    using StockPredictionMRApi.DbContext;
    using Microsoft.EntityFrameworkCore;

    public class PredictionModel
    {
        private readonly MLContext _mlContext;
        private ITransformer? _model;
        private readonly StockDbContext _dbContext;

        public PredictionModel(StockDbContext dbContext)
        {
            _mlContext = new MLContext();
            _dbContext = dbContext;
            _model = null;
        }

        // Inicjalizacja modelu
        public async Task InitializeModelAsync(string symbol)
        {
            _model = await TrainModelAsync(symbol);
        }

        // Trenowanie modelu na danych z bazy
        private async Task<ITransformer> TrainModelAsync(string symbol)
        {
            var cryptoData = await _dbContext.CryptoData
                .Where(c => c.Symbol == symbol)
                .OrderBy(c => c.Date)
                .ToListAsync();

            if (cryptoData.Count == 0)
            {
                throw new InvalidOperationException("No data available to train the model.");
            }

            var dataView = _mlContext.Data.LoadFromEnumerable(cryptoData);

            var pipeline = _mlContext.Transforms.Concatenate("Features", "Open", "High", "Low", "Volume")
                .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "Close", featureColumnName: "Features"));

            return pipeline.Fit(dataView);
        }

        // Przewidywanie serii danych
        public List<(DateTime Date, float PredictedPrice)> PredictSeries(List<CryptoDataEntity> input, int steps)
        {
            if (_model == null)
                throw new InvalidOperationException("Model is not initialized.");

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<CryptoDataEntity, CryptoPrediction>(_model);
            var predictions = new List<(DateTime Date, float PredictedPrice)>();

            // Ostatni rekord jako punkt startowy
            var lastRecord = input.Last();

            for (int i = 0; i < steps; i++)
            {
                // Przewidujemy cenę zamknięcia
                var predictedPrice = predictionEngine.Predict(lastRecord).Score;

                // Dodajemy prognozę do listy
                predictions.Add((lastRecord.Date.AddDays(1), predictedPrice));

                // Aktualizujemy dane dla kolejnej prognozy
                var random = new Random();
                lastRecord = new CryptoDataEntity
                {
                    Date = lastRecord.Date.AddDays(1),        // Nowa data
                    Open = predictedPrice * (1 + (float)((random.NextDouble() - 0.5) * 0.02)), // Mała zmienność (±1%)
                    High = predictedPrice * (1.02f + (float)((random.NextDouble() - 0.5) * 0.02)), // Wyższa zmienność (±2%)
                    Low = predictedPrice * (0.98f + (float)((random.NextDouble() - 0.5) * 0.02)),  // Niższa zmienność (±2%)
                    Close = predictedPrice,                  // Cena zamknięcia to prognoza
                    Volume = lastRecord.Volume * (1 + (float)(random.NextDouble() - 0.5) * 0.1f), // Zmienność wolumenu (±10%)
                    Symbol = lastRecord.Symbol               // Symbol pozostaje bez zmian
                };
            }

            return predictions;
        }
    }
}

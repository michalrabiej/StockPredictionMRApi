namespace StockPredictionMRApi.DbContext
{
    using Microsoft.EntityFrameworkCore;
    using StockPredictionMRApi.Models;

    public class StockDbContext : DbContext
    {
        public DbSet<CryptoDataEntity> CryptoData { get; set; }

        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }
    }
}

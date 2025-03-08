using Microsoft.EntityFrameworkCore;
using StockPredictionMRApi;
using StockPredictionMRApi.DbContext;
using StockPredictionMRApi.Models;
using StockPredictionMRApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(CryptoMappingProfile));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<CryptoDataService>();

builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inne rejestracje (np. ML model)
builder.Services.AddScoped<PredictionModel>();

// Dodaj konfigurację CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    dbContext.Database.Migrate(); // Automatyczne zastosowanie migracji
}

Config.ApiKey = builder.Configuration.GetValue<string>("ApiSettings:CryptoCompareKey");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Włącz CORS w aplikacji
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
namespace StockPredictionMRApi.Services
{
    using AutoMapper;
    using StockPredictionMRApi.Models;

    public class CryptoMappingProfile : Profile
    {
        public CryptoMappingProfile()
        {
            CreateMap<CryptoDataEntity, CryptoDataEntity>();
            CreateMap<CryptoDataEntity, CryptoDataEntity>();
        }
    }
}

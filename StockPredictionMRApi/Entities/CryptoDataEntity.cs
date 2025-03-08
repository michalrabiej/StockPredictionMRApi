﻿using System.ComponentModel.DataAnnotations;

namespace StockPredictionMRApi.Models
{
    public class CryptoDataEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public float Volume { get; set; }
        public string Symbol { get; set; }
    }
}

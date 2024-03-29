﻿using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs
{
    public class TruckDTO
    {
        public int Chassis { get; set; }
        public decimal KilometerCount { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Color { get; set; }
        public int TruckAxles { get; set; }
        public decimal LastMaintenanceKilometers { get; set; }
        public bool InMaintenance { get; set; }
    }
}
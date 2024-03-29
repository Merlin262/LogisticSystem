﻿using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs
{
    public class MaintenanceDTO
    {
        [JsonIgnore] public int Id { get; set; }
        public DateOnly MaintenanceDate { get; set; }
        public int FkEmployee { get; set; }
        public int FkTruckChassis { get; set; }
    }
}
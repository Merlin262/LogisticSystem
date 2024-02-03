using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Truck
{
    public int Chassis { get; set; }

    public decimal? MaximumWeight { get; set; }

    public int? KilometerCount { get; set; }

    public string? Model { get; set; }

    public int? Year { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

    public virtual ICollection<TruckDriver> TruckDrivers { get; set; } = new List<TruckDriver>();
}

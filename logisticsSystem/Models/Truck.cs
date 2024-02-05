using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Truck
{
    public int Chassis { get; set; }

    public decimal KilometerCount { get; set; }

    public string? Model { get; set; }

    public int? Year { get; set; }

    public string? Color { get; set; }

    public int TruckAxles { get; set; }

    public decimal LastMaintenanceKilometers { get; set; }

    public bool InMaintenance { get; set; }

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}

using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class MaitenanceTruckPart
{
    public int Id { get; set; }

    public int FkTruckPartId { get; set; }

    public int FkMaintenanceId { get; set; }

    public virtual Maintenance FkMaintenance { get; set; }

    public virtual TruckPart FkTruckPart { get; set; }
}

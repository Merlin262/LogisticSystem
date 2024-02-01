using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Maintenance
{
    public int Id { get; set; }

    public DateOnly? MaintenanceDate { get; set; }

    public int? FkEmployee { get; set; }

    public int? FkTruckChassis { get; set; }

    public virtual Employee? FkEmployeeNavigation { get; set; }

    public virtual Truck? FkTruckChassisNavigation { get; set; }
}

using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class TruckDriver
{
    public int? FkTruckChassis { get; set; }

    public int? FkEmployeeId { get; set; }

    public virtual Employee? FkEmployee { get; set; }

    public virtual Truck? FkTruckChassisNavigation { get; set; }
}

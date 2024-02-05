using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class TruckPart
{
    public int Id { get; set; }

    public string Description { get; set; }

    public virtual ICollection<MaitenanceTruckPart> MaitenanceTruckParts { get; set; } = new List<MaitenanceTruckPart>();
}

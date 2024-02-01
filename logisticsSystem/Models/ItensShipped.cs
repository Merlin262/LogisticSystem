using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class ItensShipped
{
    public int? FkItensStockId { get; set; }

    public int? FkShippingId { get; set; }

    public virtual ItensStock? FkItensStock { get; set; }

    public virtual Shipping? FkShipping { get; set; }
}

using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class ItensStock
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<ItensShipped> ItensShippeds { get; set; } = new List<ItensShipped>();
}

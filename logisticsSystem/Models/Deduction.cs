using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace logisticsSystem.Models;

public partial class Deduction
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }
    [JsonIgnore]
    public virtual ICollection<WageDeduction> WageDeductions { get; set; } = new List<WageDeduction>();
}

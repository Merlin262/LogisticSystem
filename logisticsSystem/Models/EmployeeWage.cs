using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class EmployeeWage
{
    public int Id { get; set; }

    public DateOnly PayDay { get; set; }

    public decimal Amount { get; set; }

    public decimal ComissionPercentage { get; set; }

    public decimal? ComissionPercentage { get; set; }

    public decimal? Commission { get; set; }

    public virtual Employee FkEmployee { get; set; }

    public virtual ICollection<WageDeduction> WageDeductions { get; set; } = new List<WageDeduction>();
}

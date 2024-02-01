using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class WageDeduction
{
    public int? FkDeductionsId { get; set; }

    public int? FkWageId { get; set; }

    public virtual Deduction? FkDeductions { get; set; }

    public virtual EmployeeWage? FkWage { get; set; }
}

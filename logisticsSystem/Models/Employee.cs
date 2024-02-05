using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Employee
{
    public int FkPersonId { get; set; }

    public string Position { get; set; }

    public virtual ICollection<EmployeeWage> EmployeeWages { get; set; } = new List<EmployeeWage>();

    public virtual Person FkPerson { get; set; } = null!;

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}

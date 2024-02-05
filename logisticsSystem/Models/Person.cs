using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Person
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public DateOnly BirthDate { get; set; }

    public string Email { get; set; }

    public int FkAddressId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Address? FkAddress { get; set; }
}

using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Phone
{
    public int Id { get; set; }

    public string? AreaCode { get; set; }

    public string? Number { get; set; }

    public int? FkPersonId { get; set; }

    public virtual Person? FkPerson { get; set; }
}

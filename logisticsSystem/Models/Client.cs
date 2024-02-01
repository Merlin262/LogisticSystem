using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Client
{
    public int FkPersonId { get; set; }

    public virtual Person FkPerson { get; set; } = null!;

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}

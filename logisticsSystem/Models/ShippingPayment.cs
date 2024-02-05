using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class ShippingPayment
{
    public int Id { get; set; }

    public DateOnly PaymentDate { get; set; }

    public int FkShippingId { get; set; }

    public virtual Shipping FkShipping { get; set; }
}

using System;
using System.Collections.Generic;

namespace logisticsSystem.Models;

public partial class Shipping
{
    public DateOnly? SendDate { get; set; }

    public DateOnly? EstimatedDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public decimal? TotalWeight { get; set; }

    public decimal? DistanceKm { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public int Id { get; set; }

    public decimal? ShippingPrice { get; set; }

    public int? FkClientId { get; set; }

    public int? FkEmployeeId { get; set; }

    public int? FkAddressId { get; set; }

    public virtual Address? FkAddress { get; set; }

    public virtual Client? FkClient { get; set; }

    public virtual Employee? FkEmployee { get; set; }

    public virtual ICollection<ShippingPayment> ShippingPayments { get; set; } = new List<ShippingPayment>();
}

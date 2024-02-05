using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs;

public class ShippingPaymentDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public DateOnly PaymentDate { get; set; }
    public int FkShippingId { get; set; }

    //public ShippingDTO FkShipping { get; set; }
}

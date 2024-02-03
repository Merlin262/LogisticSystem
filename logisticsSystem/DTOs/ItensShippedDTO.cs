using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;

public class ItensShippedDTO
{
    public int? FkItensStockId { get; set; }
    public int? FkShippingId { get; set; }

    public ItensStockDTO FkItensStock { get; set; }
    public ShippingDTO FkShipping { get; set; }
}

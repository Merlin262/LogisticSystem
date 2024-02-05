using logisticsSystem.Models;
using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;

public class ItensShippedDTO
{
    public int Id { get; set; }
    public int FkItensStockId { get; set; }
    public int FkShippingId { get; set; }

    public int QuantityItens { get; set; }
}

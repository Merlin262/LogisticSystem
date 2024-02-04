using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs
{
    public class ItensStockDTO
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
    }
}
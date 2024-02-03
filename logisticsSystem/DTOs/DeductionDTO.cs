using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;

public class DeductionDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal? Amount { get; set; }
    public string? Description { get; set; }
}

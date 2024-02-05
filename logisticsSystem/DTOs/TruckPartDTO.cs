using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs;

public class TruckPartDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Description { get; set; }
}

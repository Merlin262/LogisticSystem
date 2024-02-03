using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs;

public class WageDeductionDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public int? FkDeductionsId { get; set; }
    public int? FkWageId { get; set; }
}
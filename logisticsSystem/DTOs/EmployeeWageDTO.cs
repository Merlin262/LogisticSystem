using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;

public class EmployeeWageDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public DateOnly? PayDay { get; set; }
    public decimal? Amount { get; set; }
    public int? FkEmployeeId { get; set; }

    //public EmployeeDTO FkEmployee { get; set; }
}

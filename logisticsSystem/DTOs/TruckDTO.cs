namespace logisticsSystem.DTOs;

public class TruckDTO
{
    public int Chassis { get; set; }
    public decimal? MaximumWeight { get; set; }
    public int? KilometerCount { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }

}
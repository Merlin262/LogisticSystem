namespace logisticsSystem.DTOs;

public class WageDeductionDTO
{
    public int Id { get; set; }
    public int? FkDeductionsId { get; set; }
    public int? FkWageId { get; set; }
}
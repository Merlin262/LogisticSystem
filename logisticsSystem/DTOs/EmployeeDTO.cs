namespace logisticsSystem.DTOs;

public class EmployeeDTO
{
    public int FkPersonId { get; set; }
    public string? Position { get; set; }
    public decimal? Commission { get; set; }

    public PersonDTO FkPerson { get; set; }

}

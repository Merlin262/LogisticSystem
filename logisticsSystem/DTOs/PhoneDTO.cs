namespace logisticsSystem.DTOs;

public class PhoneDTO
{
    public int Id { get; set; }
    public string? AreaCode { get; set; }
    public string? Number { get; set; }
    public int? FkPersonId { get; set; }

    public PersonDTO FkPerson { get; set; }
}

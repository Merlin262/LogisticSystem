namespace logisticsSystem.DTOs;

public class ClientDTO
{
    public int FkPersonId { get; set; }

    public PersonDTO FkPerson { get; set; }
}

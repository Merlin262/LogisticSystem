using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs;

public class PersonDTO
{
    [JsonIgnore]
    public int Id { get; set; }

    public string? Name { get; set; }

    //public DateOnly? BirthDate { get; set; }

    public string? Email { get; set; }

    //public int? FkAddressId { get; set; }

    //public AddressDTO? FkAddress { get; set; }

    //public ICollection<PhoneDTO> Phones { get; set; } = new List<PhoneDTO>();
}

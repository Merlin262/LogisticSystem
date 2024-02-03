using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs;

public class ClientDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public AddressDTO FkAddress { get; set; }
    //public List<PhoneDTO> Phones { get; set; }
    //public string Complement { get; set; }
    //public string ZipCode { get; set; }
}

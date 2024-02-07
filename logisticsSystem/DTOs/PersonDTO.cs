using System.Text.Json.Serialization;

using logisticsSystem.Models;

namespace logisticsSystem.DTOs
{
    public class PersonDTO
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }
        public string? CPF { get; set; }
        public DateOnly BirthDate { get; set; }

        public string Email { get; set; }

        public int FkAddressId { get; set; }
    }
}
using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs
{
    public class EmployeeDTO
    {
        public int FkPersonId { get; set; }

        public string Position { get; set; }
    }
}
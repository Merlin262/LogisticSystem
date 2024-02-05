using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;

public class EmployeeDTO
{
    public int FkPersonId { get; set; }

    public string Position { get; set; }

    public decimal Commission { get; set; }

    //public List<EmployeeWageDTO> EmployeeWages { get; set; } = new List<EmployeeWageDTO>();

    //public PersonDTO FkPerson { get; set; } = new PersonDTO(); // Certifique-se de criar a classe PersonDTO

    //public List<MaintenanceDTO> Maintenances { get; set; } = new List<MaintenanceDTO>();

    //public List<ShippingDTO> Shippings { get; set; } = new List<ShippingDTO>();

    //public List<TruckDriverDTO> TruckDrivers { get; set; } = new List<TruckDriverDTO>();
}


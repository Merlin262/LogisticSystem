using System.Text.Json.Serialization;

namespace logisticsSystem.DTOs;

public class TruckDriverDTO
{
    public int FkTruckChassis { get; set; }
    public int FkEmployeeId { get; set; }

    //public EmployeeDTO FkEmployee { get; set; }
    //public TruckDTO FkTruckChassisNavigation { get; set; }
}

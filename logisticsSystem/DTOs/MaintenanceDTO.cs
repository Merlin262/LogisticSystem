namespace logisticsSystem.DTOs;

public class MaintenanceDTO
{
    public int Id { get; set; }
    public DateOnly? MaintenanceDate { get; set; }
    public int? FkEmployee { get; set; }
    public int? FkTruckChassis { get; set; }

    public EmployeeDTO FkEmployeeNavigation { get; set; }
    public TruckDTO FkTruckChassisNavigation { get; set; }
}

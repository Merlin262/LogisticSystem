using System.Text.Json.Serialization;


namespace logisticsSystem.DTOs;

public class MaintenanceTruckPartDTO
{
    public int FkTruckPartId { get; set; }
    public int FkMaintenanceId { get; set; }

    //public MaintenanceDTO FkMaintenance { get; set; }
    //public TruckPartDTO FkTruckPart { get; set; }
}

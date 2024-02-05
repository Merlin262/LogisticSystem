using logisticsSystem.Data;
using logisticsSystem.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Queries : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public Queries(LogisticsSystemContext context)
        {
            _context = context;
        }

        [HttpGet("trucks-in-maintenance")]
        public IActionResult GetTrucksInMaintenance()
        {
            var trucksInMaintenance = _context.Trucks
                .Where(t => t.InMaintenance)
                .Select(t => new TruckDTO
                {
                    Chassis = t.Chassis,
                    KilometerCount = t.KilometerCount,
                    Model = t.Model,
                    Year = t.Year,
                    Color = t.Color,
                    TruckAxles = t.TruckAxles,
                    InMaintenance = t.InMaintenance,
                    LastMaintenanceKilometers = t.LastMaintenanceKilometers
                })
                .ToList();

            return Ok(trucksInMaintenance);
        }


        [HttpGet("employee-total-commission/{employeeId}")]
        public IActionResult GetEmployeeTotalCommission(int employeeId)
        {
            var totalCommission = _context.EmployeeWages
                .Where(e => e.FkEmployeeId == employeeId)
                .GroupBy(e => e.FkEmployeeId)
                .Select(g => new { FkEmployeeId = g.Key, TotalCommission = g.Sum(e => e.Commission) })
                .FirstOrDefault();

            return Ok(totalCommission);
        }


    }
}

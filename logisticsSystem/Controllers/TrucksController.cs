using logisticsSystem.Data;

namespace logisticsSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using global::logisticsSystem.DTOs;
    using global::logisticsSystem.Models;
    using global::logisticsSystem.Exceptions;
    using global::logisticsSystem.Services;

    namespace logisticsSystem.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class TrucksController : ControllerBase
        {
            private readonly LogisticsSystemContext _context;
            private readonly LoggerService _logger;

            public TrucksController(LogisticsSystemContext context, LoggerService logger)
            {
                _context = context;
                _logger = logger;
            }


            // GET geral para Truck
            [HttpGet]
            public IActionResult GetTrucks()
            {
                var trucks = _context.Trucks.ToList();

                if (trucks == null || !trucks.Any())
                {
                    throw new NotFoundException("Nenhum caminhão encontrado.");
                }

                var truckDTOs = trucks.Select(t => new TruckDTO
                {
                    Chassis = t.Chassis,
                    KilometerCount = t.KilometerCount,
                    Model = t.Model,
                    Year = t.Year,
                    Color = t.Color,
                    TruckAxles = t.TruckAxles,
                    LastMaintenanceKilometers = t.LastMaintenanceKilometers,
                    InMaintenance = t.InMaintenance
                });

                return Ok(truckDTOs);
            }

            // GET para Truck por chassi
            [HttpGet("{chassis}")]
            public IActionResult GetTruckByChassis(int chassis)
            {
                var truck = _context.Trucks.FirstOrDefault(t => t.Chassis == chassis);

                if (truck == null)
                {
                    throw new NotFoundException($"Truck com chassi: {chassis} não encontrado.");
                }

                var truckDTO = new TruckDTO
                {
                    Chassis = truck.Chassis,
                    KilometerCount = truck.KilometerCount,
                    Model = truck.Model,
                    Year = truck.Year,
                    Color = truck.Color,
                    TruckAxles = truck.TruckAxles,
                    LastMaintenanceKilometers = truck.LastMaintenanceKilometers,
                    InMaintenance = truck.InMaintenance
                };

                return Ok(truckDTO);
            }


            [HttpPut("{chassis}")]
            public async Task<IActionResult> PutTruck(int chassis, [FromBody] TruckDTO truckDTO)
            {
                var truck = await _context.Trucks.FindAsync(chassis);

                if (truck == null)
                {
                    throw new NotFoundException($"Caminhão com chassi {chassis} não encontrado.");
                }

                if (truckDTO.Chassis == 0 || string.IsNullOrWhiteSpace(truckDTO.Model) || truckDTO.Year == 0)
                {
                    throw new InvalidDataTypeException("Os atributos do caminhão são inválidos.");
                }

                truck.Chassis = truckDTO.Chassis;
                truck.TruckAxles = truckDTO.TruckAxles;
                truck.KilometerCount = truckDTO.KilometerCount;
                truck.Model = truckDTO.Model;
                truck.Year = truckDTO.Year;
                truck.Color = truckDTO.Color;
                truck.LastMaintenanceKilometers = truckDTO.LastMaintenanceKilometers;
                truck.InMaintenance = truckDTO.InMaintenance;

                _context.Entry(truck).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.WriteLogData($"Truck chassis {chassis} updated successfully.");

                return NoContent();
            }


            [HttpPost]
            public async Task<ActionResult<TruckDTO>> PostTruck([FromBody] TruckDTO truckDTO)
            {
                // Verificar se os atributos estão presentes e não nulos
                if (truckDTO.Chassis == 0 || string.IsNullOrWhiteSpace(truckDTO.Model) || truckDTO.Year == 0)
                {
                    throw new InvalidDataTypeException("Os atributos do caminhão são inválidos.");
                }

                var truck = new Truck
                {
                    Chassis = truckDTO.Chassis,
                    TruckAxles = truckDTO.TruckAxles,
                    KilometerCount = truckDTO.KilometerCount,
                    Model = truckDTO.Model,
                    Year = truckDTO.Year,
                    Color = truckDTO.Color,
                    LastMaintenanceKilometers = truckDTO.LastMaintenanceKilometers,
                    InMaintenance = truckDTO.InMaintenance
                };

                _context.Trucks.Add(truck);
                _context.SaveChanges();
                _logger.WriteLogData($"Truck chassis {truck.Chassis} recorded successfully.");

                var createdTruckDTO = new TruckDTO
                {
                    Chassis = truck.Chassis,
                    TruckAxles = truck.TruckAxles,
                    KilometerCount = truck.KilometerCount,
                    Model = truck.Model,
                    Year = truck.Year,
                    Color = truck.Color,
                    LastMaintenanceKilometers = truckDTO.LastMaintenanceKilometers,
                    InMaintenance = truckDTO.InMaintenance
                };

                return Ok(createdTruckDTO);
            }


            [HttpDelete("{chassis}")]
            public async Task<IActionResult> DeleteTruck(int chassis)
            {
                var truck = await _context.Trucks.FindAsync(chassis);
                if (truck == null)
                {
                    throw new NotFoundException($"Truck de chassi: {chassis} não encontrado no banco de dados.");
                }

                _context.Trucks.Remove(truck);

                await _context.SaveChangesAsync();
                _logger.WriteLogData($"Truck de chassis {chassis} deletado com sucesso.");

                return NoContent();
            }
        }
    }
}


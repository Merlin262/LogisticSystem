using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using logisticsSystem.Data;
using logisticsSystem.Models;
using logisticsSystem.DTOs;
using logisticsSystem.Exceptions;
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenancesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public MaintenancesController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        

        // GET geral para Maintenance
        [HttpGet]
        public IActionResult GetAllMaintenance()
        {
            var maintenances = _context.Maintenances.ToList();

            if (maintenances == null || maintenances.Count == 0)
            {
                throw new NotFoundException("Nenhuma manutenção encontrada.");
            }

            var maintenanceDtoList = maintenances.Select(m => new MaintenanceDTO
            {
                Id = m.Id,
                MaintenanceDate = m.MaintenanceDate,
                FkEmployee = m.FkEmployee,
                FkTruckChassis = m.FkTruckChassis
            }).ToList();

            return Ok(maintenanceDtoList);

        }


        // GET para Maintenance por id
        [HttpGet("{id}")]
        public IActionResult GetMaintenanceById(int id)
        {
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            if (maintenance == null)
            {
                throw new NotFoundException($"Maintenance de Id: {id} não encontrada.");
            }

            var maintenanceDto = new MaintenanceDTO
            {
                Id = maintenance.Id,
                MaintenanceDate = maintenance.MaintenanceDate,
                FkEmployee = maintenance.FkEmployee,
                FkTruckChassis = maintenance.FkTruckChassis
            };

            return Ok(maintenanceDto);
        }


        [HttpPost]
        public IActionResult CreateMaintenance([FromBody] MaintenanceDTO maintenanceDTO)
        {
            if (maintenanceDTO == null)
            {
                throw new NullRequestException("Solicitação inválida para criação de manutenção.");
            }

            if (maintenanceDTO.MaintenanceDate == default)
            {
                throw new InvalidDataTypeException("Data de manutenção inválida.");
            }

            var employeeExists = _context.Employees.Any(e => e.FkPersonId == maintenanceDTO.FkEmployee);
            if (!employeeExists)
            {
                throw new NotFoundException("Funcionário associado à manutenção não encontrado.");
            }

            var truckChassisExists = _context.Trucks.Any(tc => tc.Chassis == maintenanceDTO.FkTruckChassis);

            if (!truckChassisExists)
            {
                throw new NotFoundException("Chassi do caminhão associado à manutenção não encontrado.");
            }

            var newMaintenance = new Maintenance
            {
                MaintenanceDate = maintenanceDTO.MaintenanceDate,
                FkEmployee = maintenanceDTO.FkEmployee,
                FkTruckChassis = maintenanceDTO.FkTruckChassis
            };

            _context.Maintenances.Add(newMaintenance);

            var truckToUpdate = _context.Trucks.FirstOrDefault(tc => tc.Chassis == maintenanceDTO.FkTruckChassis);

            if (truckToUpdate != null)
            {
                // Atualizar a kilometragem pós última manutenção do caminhão
                truckToUpdate.LastMaintenanceKilometers = 0; 
                // Atualiza o status do caminhão, indicando que ele não necessita mais de manutenção
                truckToUpdate.InMaintenance = false;
            }

            _context.SaveChanges();

            return Ok(newMaintenance);
            
        }



        [HttpPut("{id}")]
        public IActionResult UpdateMaintenance(int id, [FromBody] MaintenanceDTO maintenanceDTO)
        {

            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            if (maintenance == null)
            {
                throw new NotFoundException($"Maintenance de Id: {id} não encontrada no banco de dados.");
            }

            if (maintenanceDTO.MaintenanceDate == default(DateOnly))
            {
                throw new InvalidDataException("A data da manutenção não pode ser nula ou padrão.");
            }

            var truck = _context.Trucks.FirstOrDefault(t => t.Chassis == maintenanceDTO.FkTruckChassis);

            if (truck == null)
            {
                throw new NotFoundException($"Caminhão de chassi: {maintenanceDTO.FkTruckChassis} não encontrado no banco de dados.");
            }

            maintenance.MaintenanceDate = maintenanceDTO.MaintenanceDate;
            maintenance.FkEmployee = maintenanceDTO.FkEmployee;
            maintenance.FkTruckChassis = maintenanceDTO.FkTruckChassis;

            _context.SaveChanges();
            _logger.WriteLogData($"Maintenance de id: {id} atualizado com sucesso.");

            return Ok(maintenance);

        }


        [HttpDelete("{id}")]
        public IActionResult DeleteMaintenance(int id)
        {
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);
            
            if (maintenance == null)
            {
                throw new NotFoundException($"Maintenance de Id: {id} não encontrada no banco de dados.");
            }

            _context.Maintenances.Remove(maintenance);

            _context.SaveChanges();
            _logger.WriteLogData($"Maintenance id {id} deleted successfully.");

            return NoContent(); 
        }
    }
}

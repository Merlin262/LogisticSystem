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

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenancesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public MaintenancesController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // CREATE - Método POST para Maintenance
        [HttpPost]
        public IActionResult CreateMaintenance([FromBody] MaintenanceDTO maintenanceDTO)
        {
            // Mapear MaintenanceDTO para a entidade Maintenance
            var newMaintenance = new Maintenance
            {
                MaintenanceDate = maintenanceDTO.MaintenanceDate,
                FkEmployee = maintenanceDTO.FkEmployee,
                FkTruckChassis = maintenanceDTO.FkTruckChassis
            };

            // Adicionar a nova manutenção ao contexto
            _context.Maintenances.Add(newMaintenance);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar a nova manutenção criada
            return Ok(newMaintenance);
        }

        // READ - Método GET (Todos) para Maintenance
        [HttpGet]
        public IActionResult GetAllMaintenance()
        {
            // Obter todas as manutenções
            var maintenances = _context.Maintenances.ToList();

            // Mapear Maintenance para MaintenanceDTO
            var maintenanceDtoList = maintenances.Select(m => new MaintenanceDTO
            {
                Id = m.Id,
                MaintenanceDate = m.MaintenanceDate,
                FkEmployee = m.FkEmployee,
                FkTruckChassis = m.FkTruckChassis 
            }).ToList();

            return Ok(maintenanceDtoList);
        }

        // READ - Método GET por Id para Maintenance
        [HttpGet("{id}")]
        public IActionResult GetMaintenanceById(int id)
        {
            // Obter a manutenção com o Id fornecido
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            if (maintenance == null)
            {
                return NotFound(); // Retorna 404 Not Found se a manutenção não for encontrada
            }

            // Mapear Maintenance para MaintenanceDTO
            var maintenanceDto = new MaintenanceDTO
            {
                Id = maintenance.Id,
                MaintenanceDate = maintenance.MaintenanceDate,
                FkEmployee = maintenance.FkEmployee,
                FkTruckChassis = maintenance.FkTruckChassis
            };

            return Ok(maintenanceDto);
        }

        // UPDATE - Método PUT para Maintenance
        [HttpPut("{id}")]
        public IActionResult UpdateMaintenance(int id, [FromBody] MaintenanceDTO maintenanceDTO)
        {
            // Obter a manutenção com o Id fornecido
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            if (maintenance == null)
            {
                return NotFound(); // Retorna 404 Not Found se a manutenção não for encontrada
            }

            // Atualizar propriedades da manutenção
            maintenance.MaintenanceDate = maintenanceDTO.MaintenanceDate;
            maintenance.FkEmployee = maintenanceDTO.FkEmployee;
            maintenance.FkTruckChassis = maintenanceDTO.FkTruckChassis;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(maintenance);
        }

        // DELETE - Método DELETE para Maintenance
        [HttpDelete("{id}")]
        public IActionResult DeleteMaintenance(int id)
        {
            // Obter a manutenção com o Id fornecido
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            if (maintenance == null)
            {
                return NotFound(); // Retorna 404 Not Found se a manutenção não for encontrada
            }

            // Remover a manutenção do contexto
            _context.Maintenances.Remove(maintenance);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

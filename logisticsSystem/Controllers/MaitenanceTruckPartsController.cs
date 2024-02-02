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
    public class MaitenanceTruckPartsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public MaitenanceTruckPartsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // CREATE - Método POST para MaintenanceTruckPart
        [HttpPost]
        public IActionResult CreateMaintenanceTruckPart([FromBody] MaintenanceTruckPartDTO maintenanceTruckPartDTO)
        {
            // Mapear MaintenanceTruckPartDTO para a entidade MaintenanceTruckPart
            var newMaintenanceTruckPart = new MaitenanceTruckPart()
            {
                FkTruckPartId = maintenanceTruckPartDTO.FkTruckPartId,
                FkMaintenanceId = maintenanceTruckPartDTO.FkMaintenanceId
            };

            // Adicionar o novo relacionamento ao contexto
            _context.MaitenanceTruckParts.Add(newMaintenanceTruckPart);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o novo relacionamento criado
            return Ok(newMaintenanceTruckPart);
        }

        // READ - Método GET (Todos) para MaintenanceTruckPart
        [HttpGet]
        public IActionResult GetAllMaintenanceTruckPart()
        {
            // Obter todos os relacionamentos
            var maintenanceTruckParts = _context.MaitenanceTruckParts.ToList();

            // Mapear MaintenanceTruckPart para MaintenanceTruckPartDTO
            var maintenanceTruckPartDtoList = maintenanceTruckParts.Select(mtp => new MaintenanceTruckPartDTO
            {
                FkTruckPartId = mtp.FkTruckPartId,
                FkMaintenanceId = mtp.FkMaintenanceId
            }).ToList();

            return Ok(maintenanceTruckPartDtoList);
        }

        // READ - Método GET por FkMaintenanceId para MaintenanceTruckPart
        [HttpGet("{fkMaintenanceId}")]
        public IActionResult GetMaintenanceTruckPartByFkMaintenanceId(int fkMaintenanceId)
        {
            // Obter os relacionamentos com o FkMaintenanceId fornecido
            var maintenanceTruckParts = _context.MaitenanceTruckParts
                .Where(mtp => mtp.FkMaintenanceId == fkMaintenanceId)
                .ToList();

            // Mapear MaintenanceTruckPart para MaintenanceTruckPartDTO
            var maintenanceTruckPartDtoList = maintenanceTruckParts.Select(mtp => new MaintenanceTruckPartDTO
            {
                FkTruckPartId = mtp.FkTruckPartId,
                FkMaintenanceId = mtp.FkMaintenanceId
            }).ToList();

            return Ok(maintenanceTruckPartDtoList);
        }

        // UPDATE - Método PUT para MaintenanceTruckPart
        [HttpPut("{fkMaintenanceId}")]
        public IActionResult UpdateMaintenanceTruckPart(int fkMaintenanceId, [FromBody] MaintenanceTruckPartDTO maintenanceTruckPartDTO)
        {
            // Obter o relacionamento com o FkMaintenanceId fornecido
            var maintenanceTruckPart = _context.MaitenanceTruckParts
                .FirstOrDefault(mtp => mtp.FkMaintenanceId == fkMaintenanceId);

            if (maintenanceTruckPart == null)
            {
                return NotFound(); // Retorna 404 Not Found se o relacionamento não for encontrado
            }

            // Atualizar propriedades do relacionamento
            maintenanceTruckPart.FkTruckPartId = maintenanceTruckPartDTO.FkTruckPartId;
            maintenanceTruckPart.FkMaintenanceId = maintenanceTruckPartDTO.FkMaintenanceId;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(maintenanceTruckPart);
        }

        // DELETE - Método DELETE para MaintenanceTruckPart
        [HttpDelete("{fkMaintenanceId}")]
        public IActionResult DeleteMaintenanceTruckPart(int fkMaintenanceId)
        {
            // Obter o relacionamento com o FkMaintenanceId fornecido
            var maintenanceTruckPart = _context.MaitenanceTruckParts
                .FirstOrDefault(mtp => mtp.FkMaintenanceId == fkMaintenanceId);

            if (maintenanceTruckPart == null)
            {
                return NotFound(); // Retorna 404 Not Found se o relacionamento não for encontrado
            }

            // Remover o relacionamento do contexto
            _context.MaitenanceTruckParts.Remove(maintenanceTruckPart);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

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
            // Verificar se a solicitação é nula
            if (maintenanceTruckPartDTO == null)
            {
                throw new NullRequestException("Solicitação inválida para criação de relacionamento entre manutenção e peça de caminhão.");
            }

            // Verificar se a manutenção associada ao relacionamento existe
            var maintenanceExists = _context.Maintenances.Any(m => m.Id == maintenanceTruckPartDTO.FkMaintenanceId);
            if (!maintenanceExists)
            {
                throw new NotFoundException("Manutenção associada ao relacionamento não encontrada.");
            }

            // Verificar se a peça de caminhão associada ao relacionamento existe
            var truckPartExists = _context.TruckParts.Any(tp => tp.Id == maintenanceTruckPartDTO.FkTruckPartId);
            if (!truckPartExists)
            {
                throw new NotFoundException("Peça de caminhão associada ao relacionamento não encontrada.");
            }

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

            // Verificar se há relacionamentos no banco de dados
            if (maintenanceTruckParts == null || maintenanceTruckParts.Count == 0)
            {
                throw new NotFoundException("Nenhum relacionamento entre manutenção e peça de caminhão encontrado.");
            }

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

            // Verificar se há relacionamentos no banco de dados
            if (maintenanceTruckParts == null || maintenanceTruckParts.Count == 0)
            {
                throw new NotFoundException("Nenhum relacionamento entre manutenção e peça de caminhão encontrado para o FkMaintenanceId fornecido.");
            }

            // Mapear MaintenanceTruckPart para MaintenanceTruckPartDTO
            var maintenanceTruckPartDtoList = maintenanceTruckParts.Select(mtp => new MaintenanceTruckPartDTO
            {
                FkTruckPartId = mtp.FkTruckPartId,
                FkMaintenanceId = mtp.FkMaintenanceId
            }).ToList();

            return Ok(maintenanceTruckPartDtoList);
        }


        // UPDATE - Método PUT para MaintenanceTruckPart
        [HttpPut("{fkMaintenanceTruckPartId}")]
        public IActionResult UpdateMaintenanceTruckPart(int fkMaintenanceTruckPartId, [FromBody] MaintenanceTruckPartDTO maintenanceTruckPartDTO)
        {
            // Obter o relacionamento com o FkMaintenanceId fornecido
            var maintenanceTruckPart = _context.MaitenanceTruckParts
                .FirstOrDefault(mtp => mtp.FkMaintenanceId == fkMaintenanceTruckPartId);

            // Verificar se o relacionamento foi encontrado
            if (maintenanceTruckPart == null)
            {
                throw new NotFoundException("Relacionamento entre manutenção e peça de caminhão não encontrado.");
            }

            // Validar FkTruckPartId e FkMaintenanceId antes da atualização
            if (maintenanceTruckPartDTO.FkTruckPartId <= 0)
            {
                throw new InvalidDataException("ID de peça de caminhão inválido.");
            }

            if (maintenanceTruckPartDTO.FkMaintenanceId <= 0)
            {
                throw new InvalidDataException("ID de manutenção inválido.");
            }

            // Atualizar propriedades do relacionamento
            maintenanceTruckPart.FkTruckPartId = maintenanceTruckPartDTO.FkTruckPartId;
            maintenanceTruckPart.FkMaintenanceId = maintenanceTruckPartDTO.FkMaintenanceId;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(maintenanceTruckPart);
        }


        // DELETE - Método DELETE para MaintenanceTruckPart
        [HttpDelete("{fkMaintenanceTruckPartId}")]
        public IActionResult DeleteMaintenanceTruckPart(int fkMaintenanceTruckPartId)
        {
            // Obter o relacionamento com o FkMaintenanceId fornecido
            var maintenanceTruckPart = _context.MaitenanceTruckParts
                .FirstOrDefault(mtp => mtp.FkMaintenanceId == fkMaintenanceTruckPartId);

            // Verificar se o relacionamento foi encontrado
            if (maintenanceTruckPart == null)
            {
                throw new NotFoundException("Relacionamento entre manutenção e peça de caminhão não encontrado.");
            }

            // Remover o relacionamento do contexto
            _context.MaitenanceTruckParts.Remove(maintenanceTruckPart);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

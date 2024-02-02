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
    public class ItensShippedsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public ItensShippedsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // CREATE - Método POST para ItensShipped
        [HttpPost]
        public IActionResult CreateItensShipped([FromBody] ItensShippedDTO itensShippedDTO)
        {
            // Mapear ItensShippedDTO para a entidade ItensShipped
            var newItensShipped = new ItensShipped
            {
                FkItensStockId = itensShippedDTO.FkItensStockId,
                FkShippingId = itensShippedDTO.FkShippingId
            };

            // Adicionar o novo item enviado ao contexto
            _context.ItensShippeds.Add(newItensShipped);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o novo item enviado criado
            return Ok(newItensShipped);
        }

        // READ - Método GET (Todos) para ItensShipped
        [HttpGet]
        public IActionResult GetAllItensShipped()
        {
            // Obter todos os itens enviados
            var itensShipped = _context.ItensShippeds.ToList();

            // Mapear ItensShipped para ItensShippedDTO
            var itensShippedDto = itensShipped.Select(isd => new ItensShippedDTO
            {
                FkItensStockId = isd.FkItensStockId,
                FkShippingId = isd.FkShippingId
            }).ToList();

            return Ok(itensShippedDto);
        }

        // READ - Método GET por FkItensStockId para ItensShipped
        [HttpGet("{fkItensStockId}")]
        public IActionResult GetItensShippedByFkItensStockId(int fkItensStockId)
        {
            // Obter os itens enviados com o FkItensStockId fornecido
            var itensShipped = _context.ItensShippeds
                .Where(isd => isd.FkItensStockId == fkItensStockId)
                .ToList();

            // Mapear ItensShipped para ItensShippedDTO
            var itensShippedDto = itensShipped.Select(isd => new ItensShippedDTO
            {
                FkItensStockId = isd.FkItensStockId,
                FkShippingId = isd.FkShippingId
            }).ToList();

            return Ok(itensShippedDto);
        }

        // UPDATE - Método PUT para ItensShipped
        [HttpPut("{fkItensStockId}")]
        public IActionResult UpdateItensShipped(int fkItensStockId, [FromBody] ItensShippedDTO itensShippedDTO)
        {
            // Obter o item enviado com o FkItensStockId fornecido
            var itensShipped = _context.ItensShippeds
                .FirstOrDefault(isd => isd.FkItensStockId == fkItensStockId);

            if (itensShipped == null)
            {
                return NotFound(); // Retorna 404 Not Found se o item enviado não for encontrado
            }

            // Atualizar propriedades do item enviado
            itensShipped.FkItensStockId = itensShippedDTO.FkItensStockId;
            itensShipped.FkShippingId = itensShippedDTO.FkShippingId;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(itensShipped);
        }

        // DELETE - Método DELETE para ItensShipped
        [HttpDelete("{fkItensStockId}")]
        public IActionResult DeleteItensShipped(int fkItensStockId)
        {
            // Obter o item enviado com o FkItensStockId fornecido
            var itensShipped = _context.ItensShippeds
                .FirstOrDefault(isd => isd.FkItensStockId == fkItensStockId);

            if (itensShipped == null)
            {
                return NotFound(); // Retorna 404 Not Found se o item enviado não for encontrado
            }

            // Remover o item enviado do contexto
            _context.ItensShippeds.Remove(itensShipped);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

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
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensShippedsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly TruckService _truckService;
        private readonly ItensShippedService _itensShippedService;
        private readonly ShippingsController _shippingsController;

        public ItensShippedsController(LogisticsSystemContext context, ItensShippedService itensShippedService, TruckService truckService)
        {
            _context = context;
            _itensShippedService = itensShippedService;
            _truckService = truckService;
        }

        [HttpPost]
        public IActionResult CreateItensShipped([FromBody] ItensShippedDTO itensShippedDTO)
        {
            try
            {
                // Obter o item em ItensStock correspondente ao FkItensStockId
                var itensStockItem = _context.ItensStocks.FirstOrDefault(ist => ist.Id == itensShippedDTO.FkItensStockId);

                // Validar se há itens em estoque e se a quantidade desejada está disponível
                if (itensStockItem == null || itensStockItem.Quantity < itensShippedDTO.QuantityItens)
                {
                    return BadRequest("A quantidade desejada de itens não está disponível em ItensStock.");
                }

                // Mapear ItensShippedDTO para a entidade ItensShipped
                var newItensShipped = new ItensShipped
                {
                    Id = itensShippedDTO.Id,
                    FkItensStockId = itensShippedDTO.FkItensStockId,
                    FkShippingId = itensShippedDTO.FkShippingId,
                    QuantityItens = itensShippedDTO.QuantityItens
                };

                // Adicionar o novo item enviado ao contexto
                _context.ItensShippeds.Add(newItensShipped);
                _context.SaveChanges();

                // Calcular totalItemWeight
                decimal totalItemWeight = _itensShippedService.GetTotalItemWeight(itensShippedDTO.Id);

                // Calcular o peso dos eixos do caminhão
                decimal truckAxlesWeight = _truckService.GetTruckAxlesWeight(itensShippedDTO.FkShippingId);

                // Obter TotalWeight atual da tabela Shipping
                var shippingToUpdate = _context.Shippings.FirstOrDefault(s => s.Id == itensShippedDTO.FkShippingId);
                if (shippingToUpdate == null)
                {
                    return NotFound("Shipping not found.");
                }

                // Somar totalItemWeight ao TotalWeight
                decimal updatedTotalWeight = shippingToUpdate.TotalWeight + totalItemWeight;

                // Validar se o resultado da soma é maior que truckAxlesWeight
                if (updatedTotalWeight > truckAxlesWeight)
                {
                    // Remover ItensShipped do banco de dados em caso de BadRequest
                    DeleteItensShipped(newItensShipped.Id); // Use the correct property for the FK

                    // Delete the referenced Shipping in case of BadRequest
                    return BadRequest("A soma do peso dos itens excede o peso dos eixos do caminhão.");
                }

                // Atualizar TotalWeight na tabela Shipping
                shippingToUpdate.TotalWeight = updatedTotalWeight;
                _context.SaveChanges();

                // Decrementar a quantidade de itens em ItensStock
                itensStockItem.Quantity -= itensShippedDTO.QuantityItens;
                _context.SaveChanges();

                // Retornar o novo item enviado criado
                return Ok(new { TotalItemWeight = totalItemWeight, TruckAxlesWeight = truckAxlesWeight });
            }
            catch (Exception ex)
            {
                // Handle other exceptions if needed
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
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

        [HttpPut("{id}")]
        public IActionResult UpdateItensShipped(int id, [FromBody] ItensShippedDTO updatedItensShippedDTO)
        {
            try
            {
                // Obter o item em ItensShipped correspondente ao id
                var existingItensShipped = _context.ItensShippeds.FirstOrDefault(isd => isd.Id == id);

                // Validar se o item existe
                if (existingItensShipped == null)
                {
                    return NotFound("ItensShipped not found.");
                }

                // Obter o item em ItensStock correspondente ao FkItensStockId
                var itensStockItem = _context.ItensStocks.FirstOrDefault(ist => ist.Id == updatedItensShippedDTO.FkItensStockId);

                // Validar se há itens em estoque e se a quantidade desejada está disponível
                if (itensStockItem == null || itensStockItem.Quantity < updatedItensShippedDTO.QuantityItens)
                {
                    return BadRequest("A quantidade desejada de itens não está disponível em ItensStock.");
                }

                // Atualizar as propriedades do ItensShipped existente
                existingItensShipped.FkItensStockId = updatedItensShippedDTO.FkItensStockId;
                existingItensShipped.FkShippingId = updatedItensShippedDTO.FkShippingId;
                existingItensShipped.QuantityItens = updatedItensShippedDTO.QuantityItens;

                // Salvar as alterações no banco de dados
                _context.SaveChanges();

                // Calcular totalItemWeight
                decimal totalItemWeight = _itensShippedService.GetTotalItemWeight(existingItensShipped.Id);

                // Calcular o peso dos eixos do caminhão
                decimal truckAxlesWeight = _truckService.GetTruckAxlesWeight(existingItensShipped.FkShippingId);

                // Obter TotalWeight atual da tabela Shipping
                var shippingToUpdate = _context.Shippings.FirstOrDefault(s => s.Id == existingItensShipped.FkShippingId);
                if (shippingToUpdate == null)
                {
                    return NotFound("Shipping not found.");
                }

                // Somar totalItemWeight ao TotalWeight
                decimal updatedTotalWeight = shippingToUpdate.TotalWeight - existingItensShipped.QuantityItens + totalItemWeight;

                // Validar se o resultado da soma é maior que truckAxlesWeight
                if (updatedTotalWeight > truckAxlesWeight)
                {
                    // Reverter as alterações em caso de BadRequest
                    existingItensShipped.FkItensStockId = updatedItensShippedDTO.FkItensStockId; // Revertendo as alterações
                    existingItensShipped.FkShippingId = updatedItensShippedDTO.FkShippingId;
                    existingItensShipped.QuantityItens = updatedItensShippedDTO.QuantityItens;
                    _context.SaveChanges();

                    return BadRequest("A soma do peso dos itens excede o peso dos eixos do caminhão.");
                }

                // Atualizar TotalWeight na tabela Shipping
                shippingToUpdate.TotalWeight = updatedTotalWeight;
                _context.SaveChanges();

                // Atualizar a quantidade de itens em ItensStock
                itensStockItem.Quantity -= updatedItensShippedDTO.QuantityItens;
                _context.SaveChanges();

                // Retornar os detalhes do ItensShipped atualizado
                return Ok(new
                {
                    Id = existingItensShipped.Id,
                    FkItensStockId = existingItensShipped.FkItensStockId,
                    FkShippingId = existingItensShipped.FkShippingId,
                    QuantityItens = existingItensShipped.QuantityItens,
                    TotalItemWeight = totalItemWeight,
                    TruckAxlesWeight = truckAxlesWeight
                });
            }
            catch (Exception ex)
            {
                // Handle other exceptions if needed
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        // DELETE: api/ItensShipped/1
        [HttpDelete("{id}")]
        public IActionResult DeleteItensShipped(int id)
        {
            // Find the ItensShippedDTO entity with the given id
            var itensShipped = _context.ItensShippeds.FirstOrDefault(isd => isd.Id == id);

            if (itensShipped == null)
            {
                return NotFound(); // Return 404 Not Found if the item is not found
            }

            // Remove the item from the context
            _context.ItensShippeds.Remove(itensShipped);

            // Save changes to the database
            _context.SaveChanges();

            return NoContent(); // Return 204 No Content to indicate successful deletion
        }
    }
}

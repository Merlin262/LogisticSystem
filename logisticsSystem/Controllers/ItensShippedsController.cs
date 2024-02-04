﻿using System;
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

                // Validar se o resultado do primeiro cálculo é maior que o segundo
                if (truckAxlesWeight < totalItemWeight)
                {
                    // Remover ItensShipped do banco de dados em caso de BadRequest
                    DeleteItensShipped(itensShippedDTO.Id);
                    return BadRequest("O peso dos eixos do caminhão é maior que o peso total dos itens.");
                }

                // Salvar as alterações no banco de dados

                // Atualizar TotalWeight na tabela Shipping
                var shippingToUpdate = _context.Shippings.FirstOrDefault(s => s.Id == itensShippedDTO.FkShippingId);
                if (shippingToUpdate != null)
                {
                    shippingToUpdate.TotalWeight = totalItemWeight;
                    _context.SaveChanges();
                }

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
            itensShipped.QuantityItens = itensShippedDTO.QuantityItens;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(itensShipped);
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

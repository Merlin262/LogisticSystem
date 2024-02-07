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
using logisticsSystem.Exceptions;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensShippedsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly TruckService _truckService;
        private readonly ItensShippedService _itensShippedService;
        private readonly LoggerService _logger;

        public ItensShippedsController(LogisticsSystemContext context, ItensShippedService itensShippedService, TruckService truckService, LoggerService logger)
        {
            _context = context;
            _itensShippedService = itensShippedService;
            _truckService = truckService;
            _logger = logger;
        }


        // GET geral para ItensShipped
        [HttpGet]
        public IActionResult GetAllItensShipped()
        {
            var itensShipped = _context.ItensShippeds.ToList();

            var itensShippedDto = itensShipped.Select(isd => new ItensShippedDTO
            {
                Id = isd.Id,
                FkItensStockId = isd.FkItensStockId,
                FkShippingId = isd.FkShippingId,
                QuantityItens = isd.QuantityItens
            }).ToList();

            return Ok(itensShippedDto);
        }


        // GET para ItensShipped por id
        [HttpGet("{id}")]
        public IActionResult GetItensShippedById(int id)
        {
            var itemStockExists = _context.ItensShippeds.Any(isp => isp.Id == id);
            if (!itemStockExists)
            {
                throw new NotFoundException($"Nenhum ItensShipped com ID: {id} encontrado no banco de dados.");
            }

            var itensShipped = _context.ItensShippeds
                .Where(isd => isd.FkItensStockId == id)
                .ToList();

            var itensShippedDto = itensShipped.Select(isd => new ItensShippedDTO
            {
                Id = isd.Id,
                FkItensStockId = isd.FkItensStockId,
                FkShippingId = isd.FkShippingId,
                QuantityItens = isd.QuantityItens
            }).ToList();

            return Ok(itensShippedDto);
        }


        [HttpPost]
        public IActionResult CreateItensShipped([FromBody] ItensShippedDTO itensShippedDTO)
        {
            var itensStockItem = _context.ItensStocks.FirstOrDefault(ist => ist.Id == itensShippedDTO.FkItensStockId);

            if (itensStockItem == null || itensStockItem.Quantity < itensShippedDTO.QuantityItens)
            {
                throw new InsufficientQuantityException("A quantidade desejada de itens não está disponível em ItensStock.");
            }

            var newItensShipped = new ItensShipped
            {
                Id = itensShippedDTO.Id,
                FkItensStockId = itensShippedDTO.FkItensStockId,
                FkShippingId = itensShippedDTO.FkShippingId,
                QuantityItens = itensShippedDTO.QuantityItens
            };

            _context.ItensShippeds.Add(newItensShipped);
            _context.SaveChanges();
            _logger.WriteLogData($"Item shipped id {newItensShipped.Id} recorded successfully.");

            // Calcula o peso total de um pedido
            decimal totalItemWeight = _itensShippedService.GetTotalItemWeight(itensShippedDTO.Id);

            // Calcular o peso maximo suportado pelos eixos do caminhão
            decimal truckAxlesWeight = _truckService.GetTruckAxlesWeight(itensShippedDTO.FkShippingId);

            var shippingToUpdate = _context.Shippings.FirstOrDefault(s => s.Id == itensShippedDTO.FkShippingId);
            if (shippingToUpdate == null)
            {
                throw new NotFoundException($"Shipping com o ID: {shippingToUpdate.Id}");
            }

            // Soma os itens do pedido com os outros itens no caminhão (se existirem)
            decimal updatedTotalWeight = shippingToUpdate.TotalWeight + totalItemWeight;

            // Validar se o resultado da soma é maior que o peso maximo do caminhão
            if (updatedTotalWeight > truckAxlesWeight)
            {
                DeleteItensShipped(newItensShipped.Id);
                throw new TruckOverloadedException("A soma do peso dos itens excede o peso dos eixos do caminhão.");
            }

            // Atualizar TotalWeight na tabela Shipping
            shippingToUpdate.TotalWeight = updatedTotalWeight;
            _context.SaveChanges();

            // Decrementar a quantidade de itens em ItensStock
            itensStockItem.Quantity -= itensShippedDTO.QuantityItens;
            _context.SaveChanges();

            return Ok(new { TotalItemWeight = totalItemWeight, TruckAxlesWeight = truckAxlesWeight });
        }



        [HttpPut("{id}")]
        public IActionResult UpdateItensShipped(int id, [FromBody] ItensShippedDTO updatedItensShippedDTO)
        {

            var existingItensShipped = _context.ItensShippeds.FirstOrDefault(isd => isd.Id == id);

            if (existingItensShipped == null)
            {
                throw new NotFoundException($"ItensShipped com o ID: {id} não encontrado no banco de dados.");
            }

            // Obter o item em ItensStock correspondente ao FkItensStockId
            var itensStockItem = _context.ItensStocks.FirstOrDefault(ist => ist.Id == updatedItensShippedDTO.FkItensStockId);

            // Validar se há itens em estoque e se a quantidade desejada está disponível
            if (itensStockItem == null || itensStockItem.Quantity < updatedItensShippedDTO.QuantityItens)
            {
                throw new ItemNotAvailableInStockException("A quantidade desejada de itens não está disponível em ItensStock.");
            }

            existingItensShipped.FkItensStockId = updatedItensShippedDTO.FkItensStockId;
            existingItensShipped.FkShippingId = updatedItensShippedDTO.FkShippingId;
            existingItensShipped.QuantityItens = updatedItensShippedDTO.QuantityItens;

            _context.SaveChanges();
            _logger.WriteLogData($"Item shipped id {id} updated successfully.");

            // Calcular totalItemWeight
            decimal totalItemWeight = (decimal)_itensShippedService.GetTotalItemWeight(existingItensShipped.Id);

            // Calcular o peso dos eixos do caminhão
            decimal truckAxlesWeight = (decimal)_truckService.GetTruckAxlesWeight(existingItensShipped.FkShippingId);

            // Obter TotalWeight atual da tabela Shipping
            var shippingToUpdate = _context.Shippings.FirstOrDefault(s => s.Id == existingItensShipped.FkShippingId);
            if (shippingToUpdate == null)
            {
                throw new NotFoundException("Shipping not found.");
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

                throw new TruckOverloadedException("A soma do peso dos itens excede o peso dos eixos do caminhão.");
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


        [HttpDelete("{id}")]
        public IActionResult DeleteItensShipped(int id)
        {
            var itensShipped = _context.ItensShippeds.FirstOrDefault(isd => isd.Id == id);

            if (itensShipped == null)
            {
                throw new NotFoundException($"ItensShipped com o ID: {id} não encontrado no banco de dados.");
            }

            _context.ItensShippeds.Remove(itensShipped);

            _context.SaveChanges();

            return NoContent(); 
        }

    }
}

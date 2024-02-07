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
    public class ItensStocksController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public ItensStocksController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET geral de ItensStock
        [HttpGet]
        public IActionResult GetItensStock()
        {
            var itensStock = _context.ItensStocks.ToList();

            var itensStockDTOs = itensStock.Select(item => new ItensStockDTO
            {
                Id = item.Id,
                Description = item.Description,
                Quantity = item.Quantity,
                Price = item.Price,
                Weight = item.Weight 
            }).ToList();

            return Ok(itensStockDTOs);
        }


        // GET para ItensStock por id
        [HttpGet("{id}")]
        public IActionResult GetItensStockById(int id)
        {
            var itensStock = _context.ItensStocks.FirstOrDefault(i => i.Id == id);

            if (itensStock == null)
            {
                throw new NotFoundException($"ItensStock de ID: {id} não encontrado no banco de dados.");
            }

            var itensStockDTO = new ItensStockDTO
            {
                Id = itensStock.Id,
                Description = itensStock.Description,
                Quantity = itensStock.Quantity,
                Price = itensStock.Price,
                Weight = itensStock.Weight
            };

            return Ok(itensStockDTO);
        }

        [HttpPost]
        public IActionResult CreateItensStock([FromBody] ItensStockDTO itensStockDTO)
        {
            if (itensStockDTO == null)
            {
                throw new InvalidDataTypeException("Dados inválidos para o item de estoque.");
            }

            if (string.IsNullOrEmpty(itensStockDTO.Description))
            {
                throw new InvalidDataTypeException("A descrição do item de estoque é obrigatória.");
            }

            if (itensStockDTO.Quantity <= 0)
            {
                throw new InvalidDataTypeException("A quantidade do item de estoque deve ser maior que zero.");
            }

            if (itensStockDTO.Price <= 0)
            {
                throw new InvalidDataTypeException("O preço do item de estoque deve ser maior que zero.");
            }

            if (itensStockDTO.Weight <= 0)
            {
                throw new InvalidDataTypeException("O peso do item de estoque deve ser maior que zero.");
            }

            var itensStock = new ItensStock
            {
                Description = itensStockDTO.Description,
                Quantity = itensStockDTO.Quantity,
                Price = itensStockDTO.Price,
                Weight = itensStockDTO.Weight
            };

            _context.ItensStocks.Add(itensStock);
            _context.SaveChanges();
            _logger.WriteLogData($"itensStock de id: {itensStock.Id} registrado com sucesso.");

            return CreatedAtAction(nameof(GetItensStockById), new { id = itensStock.Id }, itensStockDTO);
        }

        // PUT: api/itensstock/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateItensStock(int id, [FromBody] ItensStockDTO itensStockDTO)
        {
            var itensStock = _context.ItensStocks.FirstOrDefault(i => i.Id == id);

            if (itensStock == null)
            {
                throw new NotFoundException($"ItensStock de ID: {id} não encontrado no banco de dados.");
            }

            if (itensStockDTO == null)
            {
                throw new InvalidDataTypeException("Dados inválidos para o item de estoque.");
            }

            if (string.IsNullOrEmpty(itensStockDTO.Description))
            {
                throw new InvalidDataTypeException("A descrição do item de estoque é obrigatória.");
            }

            if (itensStockDTO.Quantity <= 0)
            {
                throw new InvalidDataTypeException("A quantidade do item de estoque deve ser maior que zero.");
            }

            if (itensStockDTO.Price <= 0)
            {
                throw new InvalidDataTypeException("O preço do item de estoque deve ser maior que zero.");
            }

            if (itensStockDTO.Weight <= 0)
            {
                throw new InvalidDataTypeException("O peso do item de estoque deve ser maior que zero.");
            }

            itensStock.Description = itensStockDTO.Description;
            itensStock.Quantity = itensStockDTO.Quantity;
            itensStock.Price = itensStockDTO.Price;
            itensStock.Weight = itensStockDTO.Weight;

            _context.SaveChanges();
            _logger.WriteLogData($"Item de ID: {id} Atualizado com sucesso.");

            return Ok(itensStockDTO);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteItensStock(int id)
        {
            var itensStock = _context.ItensStocks.FirstOrDefault(i => i.Id == id);

            if (itensStock == null)
            {
                throw new NotFoundException($"ItensStock de ID: {id} não encontrado no banco de dados.");
            }

            _context.ItensStocks.Remove(itensStock);
            _context.SaveChanges();
            _logger.WriteLogData($"ItensStock  de id: {id} deletado com sucesso.");

            return NoContent();
        }
    }
}

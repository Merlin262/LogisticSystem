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
    public class ItensStocksController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public ItensStocksController(LogisticsSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetItensStock()
        {
            // Obter os itens de estoque
            var itensStock = _context.ItensStocks.ToList();

            // Mapear para DTO manualmente
            var itensStockDTOs = itensStock.Select(item => new ItensStockDTO
            {
                Id = item.Id,
                Description = item.Description,
                Quantity = item.Quantity,
                Price = item.Price,
                Weight = item.Weight // Certifique-se de que Weight seja do tipo decimal
            }).ToList();

            // Retornar a lista formatada como JSON
            return Ok(itensStockDTOs);
        }

        // GET: api/itensstock/{id}
        [HttpGet("{id}")]
        public IActionResult GetItensStockById(int id)
        {
            var itensStock = _context.ItensStocks.FirstOrDefault(i => i.Id == id);

            if (itensStock == null)
            {
                return NotFound("Item de estoque não encontrado.");
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

        // POST: api/itensstock
        [HttpPost]
        public IActionResult CreateItensStock([FromBody] ItensStockDTO itensStockDTO)
        {
            if (itensStockDTO == null)
            {
                return BadRequest("Dados inválidos para o item de estoque.");
            }

            var itensStock = new ItensStock
            {
                Description = itensStockDTO.Description,
                Quantity = itensStockDTO.Quantity ,
                Price = itensStockDTO.Price,
                Weight = itensStockDTO.Weight
            };

            _context.ItensStocks.Add(itensStock);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetItensStockById), new { id = itensStock.Id }, itensStockDTO);
        }

        // PUT: api/itensstock/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateItensStock(int id, [FromBody] ItensStockDTO itensStockDTO)
        {
            var itensStock = _context.ItensStocks.FirstOrDefault(i => i.Id == id);

            if (itensStock == null)
            {
                return NotFound("Item de estoque não encontrado.");
            }

            if (itensStockDTO == null)
            {
                return BadRequest("Dados inválidos para o item de estoque.");
            }

            itensStock.Description = itensStockDTO.Description;
            itensStock.Quantity = itensStockDTO.Quantity;
            itensStock.Price = itensStockDTO.Price;
            itensStock.Weight = itensStockDTO.Weight;

            _context.SaveChanges();

            return Ok(itensStockDTO);
        }

        // DELETE: api/itensstock/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteItensStock(int id)
        {
            var itensStock = _context.ItensStocks.FirstOrDefault(i => i.Id == id);

            if (itensStock == null)
            {
                return NotFound("Item de estoque não encontrado.");
            }

            _context.ItensStocks.Remove(itensStock);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

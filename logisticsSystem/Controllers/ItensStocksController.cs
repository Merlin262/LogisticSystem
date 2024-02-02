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

        // CREATE - Método POST para ItensStock
        [HttpPost]
        public IActionResult CreateItensStock([FromBody] ItensStockDTO itensStockDTO)
        {
            // Mapear ItensStockDTO para a entidade ItensStock
            var newItensStock = new ItensStock
            {
                Id = itensStockDTO.Id,
                Description = itensStockDTO.Description,
                Quantity = itensStockDTO.Quantity,
                Price = itensStockDTO.Price
            };

            // Adicionar o novo item ao estoque ao contexto
            _context.ItensStocks.Add(newItensStock);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o novo item ao estoque criado
            return Ok(newItensStock);
        }

        // READ - Método GET (Todos) para ItensStock
        [HttpGet]
        public IActionResult GetAllItensStock()
        {
            // Obter todos os itens do estoque
            var itensStock = _context.ItensStocks.ToList();

            // Mapear ItensStock para ItensStockDTO
            var itensStockDto = itensStock.Select(isd => new ItensStockDTO
            {
                Id = isd.Id,
                Description = isd.Description,
                Quantity = isd.Quantity,
                Price = isd.Price
            }).ToList();

            return Ok(itensStockDto);
        }

        // READ - Método GET por Id para ItensStock
        [HttpGet("{id}")]
        public IActionResult GetItensStockById(int id)
        {
            // Obter o item do estoque com o Id fornecido
            var itensStock = _context.ItensStocks.FirstOrDefault(isd => isd.Id == id);

            if (itensStock == null)
            {
                return NotFound(); // Retorna 404 Not Found se o item do estoque não for encontrado
            }

            // Mapear ItensStock para ItensStockDTO
            var itensStockDto = new ItensStockDTO
            {
                Id = itensStock.Id,
                Description = itensStock.Description,
                Quantity = itensStock.Quantity,
                Price = itensStock.Price
            };

            return Ok(itensStockDto);
        }

        // UPDATE - Método PUT para ItensStock
        [HttpPut("{id}")]
        public IActionResult UpdateItensStock(int id, [FromBody] ItensStockDTO itensStockDTO)
        {
            // Obter o item do estoque com o Id fornecido
            var itensStock = _context.ItensStocks.FirstOrDefault(isd => isd.Id == id);

            if (itensStock == null)
            {
                return NotFound(); // Retorna 404 Not Found se o item do estoque não for encontrado
            }

            // Atualizar propriedades do item do estoque
            itensStock.Description = itensStockDTO.Description;
            itensStock.Quantity = itensStockDTO.Quantity;
            itensStock.Price = itensStockDTO.Price;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(itensStock);
        }

        // DELETE - Método DELETE para ItensStock
        [HttpDelete("{id}")]
        public IActionResult DeleteItensStock(int id)
        {
            // Obter o item do estoque com o Id fornecido
            var itensStock = _context.ItensStocks.FirstOrDefault(isd => isd.Id == id);

            if (itensStock == null)
            {
                return NotFound(); // Retorna 404 Not Found se o item do estoque não for encontrado
            }

            // Remover o item do estoque do contexto
            _context.ItensStocks.Remove(itensStock);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

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
    public class DeductionsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public DeductionsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // CREATE - Método POST
        [HttpPost]
        public IActionResult CreateDeduction([FromBody] DeductionDTO deductionDTO)
        {
            // Mapear DeductionDTO para a entidade Deduction
            var newDeduction = new Deduction
            {
                Id = deductionDTO.Id,
                Name = deductionDTO.Name,
                Amount = deductionDTO.Amount,
                Description = deductionDTO.Description
            };

            // Adicionar a nova dedução ao contexto
            _context.Deductions.Add(newDeduction);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar a nova dedução criada
            return Ok(newDeduction);
        }

        // READ - Método GET (Todos)
        [HttpGet]
        public IActionResult GetAllDeductions()
        {
            // Obter todas as deduções
            var deductions = _context.Deductions.ToList();

            // Mapear Deduction para DeductionDTO
            var deductionsDto = deductions.Select(d => new DeductionDTO
            {
                Id = d.Id,
                Name = d.Name,
                Amount = d.Amount,
                Description = d.Description
            }).ToList();

            return Ok(deductionsDto);
        }

        // READ - Método GET por ID
        [HttpGet("{id}")]
        public IActionResult GetDeductionById(int id)
        {
            // Obter a dedução com o ID fornecido
            var deduction = _context.Deductions.Find(id);

            if (deduction == null)
            {
                return NotFound(); // Retorna 404 Not Found se a dedução não for encontrada
            }

            // Mapear Deduction para DeductionDTO
            var deductionDto = new DeductionDTO
            {
                Id = deduction.Id,
                Name = deduction.Name,
                Amount = deduction.Amount,
                Description = deduction.Description
            };

            return Ok(deductionDto);
        }

        // UPDATE - Método PUT
        [HttpPut("{id}")]
        public IActionResult UpdateDeduction(int id, [FromBody] DeductionDTO deductionDTO)
        {
            // Obter a dedução com o ID fornecido
            var deduction = _context.Deductions.Find(id);

            if (deduction == null)
            {
                return NotFound(); // Retorna 404 Not Found se a dedução não for encontrada
            }

            // Atualizar propriedades da dedução
            deduction.Name = deductionDTO.Name;
            deduction.Amount = deductionDTO.Amount;
            deduction.Description = deductionDTO.Description;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(deduction);
        }

        // DELETE - Método DELETE
        [HttpDelete("{id}")]
        public IActionResult DeleteDeduction(int id)
        {
            // Obter a dedução com o ID fornecido
            var deduction = _context.Deductions.Find(id);

            if (deduction == null)
            {
                return NotFound(); // Retorna 404 Not Found se a dedução não for encontrada
            }

            // Remover a dedução do contexto
            _context.Deductions.Remove(deduction);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

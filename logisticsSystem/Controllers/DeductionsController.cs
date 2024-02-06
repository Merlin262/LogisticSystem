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
using System.Text.RegularExpressions;
using logisticsSystem.Services;
using System.Net;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeductionsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public DeductionsController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateDeduction([FromBody] DeductionDTO deductionDTO)
        {
            if (deductionDTO == null)
            {
                throw new NullRequestException("A solicitação é nula.");
            }

            if (!IsAlpha(deductionDTO.Name))
            {
                throw new InvalidDataTypeException("O nome da dedução deve conter apenas letras.");
            }

            if (deductionDTO.Amount < 0)
            {
                throw new InvalidDataTypeException("O valor da dedução deve ser maior ou igual a zero.");
            }

            if (deductionDTO.Description.Length > 255)
            {
                throw new InvalidDataTypeException("A descrição da dedução deve ter no máximo 255 caracteres.");
            }

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
            _logger.WriteLogData($"Discount name '{newDeduction.Name}' registered succesfully.");

            // Retornar a nova dedução criada
            return Ok(newDeduction);
        }

        [HttpGet]
        public IActionResult GetAllDeductions()
        {
            // Obter todas as deduções
            var deductions = _context.Deductions.ToList();

            if (deductions == null)
            {
                throw new NotFoundException("Nenhuma dedução encontrada.");
            }

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


        [HttpGet("{id}")]
        public IActionResult GetDeductionById(int id)
        {
            // Obter a dedução com o ID fornecido
            var deduction = _context.Deductions.Find(id);

            if (deduction == null)
            {
                throw new NotFoundException($"Nenhuma Deduction encontrada com o ID {id}.");
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
                throw new NotFoundException($"Nenhuma Deduction encontrada com o ID {id}.");
            }

            // Atualizar propriedades da dedução
            deduction.Name = deductionDTO.Name;
            deduction.Amount = deductionDTO.Amount;
            deduction.Description = deductionDTO.Description;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Deductions id {id} updated successfully.");

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
                throw new NotFoundException($"Nenhuma Deduction encontrada com o ID {id}.");
            }

            // Remover a dedução do contexto
            _context.Deductions.Remove(deduction);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Deduction id {id} deleted successfully.");

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }

        private bool IsAlpha(string value)
        {
            // Use uma expressão regular para verificar se a string contém apenas letras
            return Regex.IsMatch(value, @"^[a-zA-Z]+$");
        }
    }
}

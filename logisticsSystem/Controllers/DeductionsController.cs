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



        // GET para todas as deduções
        [HttpGet]
        public IActionResult GetAllDeductions()
        {
            var deductions = _context.Deductions.ToList();

            if (deductions == null)
            {
                throw new NotFoundException("Nenhuma Deduction encontrada no banco de dados.");
            }

            var deductionsDto = deductions.Select(d => new DeductionDTO
            {
                Id = d.Id,
                Name = d.Name,
                Amount = d.Amount,
                Description = d.Description
            }).ToList();

            return Ok(deductionsDto);
        }


        // GET para dedução por ID
        [HttpGet("{id}")]
        public IActionResult GetDeductionById(int id)
        {
            var deduction = _context.Deductions.Find(id);

            if (deduction == null)
            {
                throw new NotFoundException($"Nenhuma Deduction encontrada com o ID {id}.");
            }

            var deductionDto = new DeductionDTO
            {
                Id = deduction.Id,
                Name = deduction.Name,
                Amount = deduction.Amount,
                Description = deduction.Description
            };

            return Ok(deductionDto);
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

            if (deductionDTO.Name.Length > 255)
            {
                throw new InvalidDataTypeException("O nome da dedução deve ter no máximo 255 caracteres.");
            }

            if (deductionDTO.Amount < 0)
            {
                throw new InvalidDataTypeException("O valor da dedução deve ser maior ou igual a zero.");
            }

            if (deductionDTO.Description.Length > 255)
            {
                throw new InvalidDataTypeException("A descrição da dedução deve ter no máximo 255 caracteres.");
            }

            var newDeduction = new Deduction
            {
                Name = deductionDTO.Name,
                Amount = deductionDTO.Amount,
                Description = deductionDTO.Description
            };

            _context.Deductions.Add(newDeduction);

            _context.SaveChanges();
            _logger.WriteLogData($"Dedução de nome '{newDeduction.Name}' registrado com sucesso.");

            return Ok(newDeduction);
        }



        [HttpPut("{id}")]
        public IActionResult UpdateDeduction(int id, [FromBody] DeductionDTO deductionDTO)
        {

            var deduction = _context.Deductions.Find(id);

            if (deduction == null)
            {
                throw new NotFoundException($"Não há dedução com esse ID: {id} no banco de dados.");
            }

            if (string.IsNullOrWhiteSpace(deductionDTO.Name))
            {
                throw new InvalidDataException("O campo de nome não pode ser vazio.");
            }

            if (deductionDTO.Amount < 0)
            {
                throw new InvalidDataException("Amount não pode ser maior do que zero.");
            }

            if (string.IsNullOrWhiteSpace(deductionDTO.Description))
            {
                throw new InvalidDataException("O campo de Description não pode ser vazio.");
            }

            if (deductionDTO.Name.Length > 255)
            {
                throw new InvalidDataException("Name não pode ter mais do que 255 caracteres.");
            }

            if (deductionDTO.Description.Length > 255)
            {
                throw new InvalidDataException("Description não pode ter mais do que 255 caracteres.");
            }

            deduction.Name = deductionDTO.Name;
            deduction.Amount = deductionDTO.Amount;
            deduction.Description = deductionDTO.Description;

            _context.SaveChanges();
            _logger.WriteLogData($"Deductions id {id} updated successfully.");

            return Ok(deduction);
        }

        

        [HttpDelete("{id}")]
        public IActionResult DeleteDeduction(int id)
        {
            var deduction = _context.Deductions.Find(id);

            if (deduction == null)
            {
                throw new NotFoundException($"Nenhuma Deduction encontrada com o ID {id}.");
            }

            _context.Deductions.Remove(deduction);

            _context.SaveChanges();
            _logger.WriteLogData($"Deduction id {id} deleted successfully.");

            return NoContent(); 
        }

        // Regex para verificar se a string contém apenas letras
        private bool IsAlpha(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z\s]+$");
        }
    }
}

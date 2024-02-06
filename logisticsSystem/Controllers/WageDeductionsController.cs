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
    public class WageDeductionsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public WageDeductionsController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/WageDeductions
        [HttpGet]
        public IActionResult GetWageDeductions()
        {
            var wageDeductions = _context.WageDeductions.ToList();

            var wageDeductionDTOs = wageDeductions.Select(wd => new WageDeductionDTO
            {
                FkDeductionsId = wd.FkDeductionsId,
                FkWageId = wd.FkWageId,
            }).ToList();

            return Ok(wageDeductionDTOs);
        }


        // GET: api/WageDeductions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WageDeductionDTO>> GetWageDeduction(int id)
        {
            // Obter a dedução salarial com o Id fornecido
            var wageDeduction = await _context.WageDeductions.FindAsync(id);

            // Verificar se a dedução salarial foi encontrada
            if (wageDeduction == null)
            {
                throw new NotFoundException("Dedução salarial não encontrada.");
            }

            // Mapear a entidade para o DTO
            var wageDeductionDTO = new WageDeductionDTO
            {
                FkDeductionsId = wageDeduction.FkDeductionsId,
                FkWageId = wageDeduction.FkWageId,
            };

            return wageDeductionDTO;
        }



        // PUT: api/WageDeductions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWageDeduction(int id, [FromBody] WageDeductionDTO wageDeductionDTO)
        {
            // Verificar se o ID na solicitação corresponde ao ID da dedução salarial
            if (id != wageDeductionDTO.FkDeductionsId)
            {
                throw new NotFoundException("O ID na solicitação não corresponde ao ID da dedução salarial.");
            }

            // Mapear o DTO para a entidade
            var wageDeduction = new WageDeduction
            {
                Id = id,
                FkDeductionsId = wageDeductionDTO.FkDeductionsId,
                FkWageId = wageDeductionDTO.FkWageId,
            };

            _context.Entry(wageDeduction).State = EntityState.Modified;


            await _context.SaveChangesAsync();


            // Verificar se a exceção ocorreu devido à inexistência da entidade
            if (!_context.WageDeductions.Any(w => w.Id == id))
            {
                throw new NotFoundException($"WageDeduction com ID: {id} não encontrada no banco de dados");
            }

            return NoContent();
        }

        //POST: api/WageDeductions
        [HttpPost]
        public async Task<ActionResult<WageDeduction>> PostWageDeduction([FromBody] WageDeductionDTO wageDeductionDTO)
        {
            if (wageDeductionDTO.FkDeductionsId == 0 || wageDeductionDTO.FkWageId == 0)
            {
                throw new InvalidDataTypeException("FkDeductionsId and FkWageId are required and must be valid IDs.");
            }

            var wageDeduction = new WageDeduction
            {
                FkDeductionsId = wageDeductionDTO.FkDeductionsId,
                FkWageId = wageDeductionDTO.FkWageId,
            };

            if (wageDeductionDTO.FkDeductionsId == null || wageDeductionDTO.FkWageId == null)
            {
                throw new InvalidDataTypeException("FkDeductionsId and FkWageId are required.");
            }

            _context.WageDeductions.Add(wageDeduction);
            await _context.SaveChangesAsync();
            _logger.WriteLogData($"Wage deduction of wage id {wageDeduction.FkWageId} recorded successfully.");

            return CreatedAtAction("GetWageDeduction", new { id = wageDeduction.Id }, wageDeduction);
        }



        // DELETE: api/WageDeductions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWageDeduction(int id)
        {
            var wageDeduction = await _context.WageDeductions.FindAsync(id);

            if (wageDeduction == null)
            {
                throw new NotFoundException($"WageDeduction com ID: {id} não encontrada no banco de dados");
            }

            _context.WageDeductions.Remove(wageDeduction);
            await _context.SaveChangesAsync();
            _logger.WriteLogData($"Wage Deduction id {id} deleted successfully.");

            return NoContent();
        }
    }
}
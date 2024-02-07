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

        // GET geral para WageDeduction
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


        // GET para WageDeduction por id
        [HttpGet("{id}")]
        public async Task<ActionResult<WageDeductionDTO>> GetWageDeduction(int id)
        {
            var wageDeduction = await _context.WageDeductions.FindAsync(id);

            if (wageDeduction == null)
            {
                throw new NotFoundException($"WageDeduction de Id: {id} não encontrada no banco de dados.");
            }

            var wageDeductionDTO = new WageDeductionDTO
            {
                FkDeductionsId = wageDeduction.FkDeductionsId,
                FkWageId = wageDeduction.FkWageId,
            };

            return wageDeductionDTO;
        }


        [HttpPost]
        public async Task<ActionResult<WageDeduction>> PostWageDeduction([FromBody] WageDeductionDTO wageDeductionDTO)
        {
            if (wageDeductionDTO.FkDeductionsId == 0 ||  wageDeductionDTO.FkWageId == 0)
            {
                throw new InvalidDataTypeException("FkDeductionsId e FkWageId são necessárias e devem ter IDs validos.");
            }

            var wageDeduction = new WageDeduction
            {
                FkDeductionsId = wageDeductionDTO.FkDeductionsId,
                FkWageId = wageDeductionDTO.FkWageId,
            };

            if (wageDeductionDTO.FkDeductionsId == null ||  wageDeductionDTO.FkWageId == null)
            {
                throw new InvalidDataTypeException("FkDeductionsId and FkWageId are required.");
            }

            _context.WageDeductions.Add(wageDeduction);
            await _context.SaveChangesAsync();
            _logger.WriteLogData($"Wage deduction of wage id {wageDeduction.FkWageId} recorded successfully.");

            return CreatedAtAction("GetWageDeduction", new { id = wageDeduction.Id }, wageDeduction);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutWageDeduction(int id, [FromBody] WageDeductionDTO wageDeductionDTO)
        {
            if (id != wageDeductionDTO.FkDeductionsId)
            {
                throw new NotFoundException("O ID na solicitação não corresponde ao ID de WageDeduction.");
            }

            var wageDeduction = new WageDeduction
            {
                Id = id,
                FkDeductionsId = wageDeductionDTO.FkDeductionsId,
                FkWageId = wageDeductionDTO.FkWageId,
            };

            _context.Entry(wageDeduction).State = EntityState.Modified;

            await _context.SaveChangesAsync();
                
            if (!_context.WageDeductions.Any(w => w.Id == id))
            {
                throw new NotFoundException($"WageDeduction com ID: {id} não encontrada no banco de dados");
            }

            return NoContent();
        }

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
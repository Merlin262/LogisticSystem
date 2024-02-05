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

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WageDeductionsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public WageDeductionsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/WageDeductions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WageDeduction>>> GetWageDeductions()
        {
            return await _context.WageDeductions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WageDeduction>> GetWageDeduction(int id)
        {
            // Obter a dedução salarial com o Id fornecido
            var wageDeduction = await _context.WageDeductions.FindAsync(id);

            // Verificar se a dedução salarial foi encontrada
            if (wageDeduction == null)
            {
                throw new NotFoundException("Dedução salarial não encontrada.");
            }

            return wageDeduction;
        }


        // PUT: api/WageDeductions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWageDeduction(int id, WageDeduction wageDeduction)
        {
            // Verificar se o ID na solicitação corresponde ao ID da dedução salarial
            if (id != wageDeduction.Id)
            {
                throw new NotFoundException("O ID na solicitação não corresponde ao ID da dedução salarial.");
            }

            _context.Entry(wageDeduction).State = EntityState.Modified;

            // Verificar se a dedução salarial existe antes de salvar as alterações no banco de dados
            if (!WageDeductionExists(id))
            {
                throw new NotFoundException($"WageDeduction com ID: {id} não encontrada no banco de dados");
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /*

        // POST: api/WageDeductions
        [HttpPost]
        public async Task<ActionResult<WageDeduction>> PostWageDeduction([FromBody] WageDeductionDTO wageDeductionDTO)
        {
            if (!wageDeductionDTO.FkDeductionsId || !wageDeductionDTO.FkWageId)
            {
                throw new InvalidDataTypeException("FkDeductionsId and FkWageId are required.");
            }

            var wageDeduction = new WageDeduction
            {
                FkDeductionsId = wageDeductionDTO.FkDeductionsId,
                FkWageId = wageDeductionDTO.FkWageId,
            };

            _context.WageDeductions.Add(wageDeduction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWageDeduction", new { id = wageDeduction.Id }, wageDeduction);
        }

        */
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

            return NoContent();
        }


        private bool WageDeductionExists(int id)
        {
            return _context.WageDeductions.Any(e => e.Id == id);
        }
    }
}


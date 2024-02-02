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

        // GET: api/WageDeductions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WageDeduction>> GetWageDeduction(int id)
        {
            var wageDeduction = await _context.WageDeductions.FindAsync(id);

            if (wageDeduction == null)
            {
                return NotFound();
            }

            return wageDeduction;
        }

        // PUT: api/WageDeductions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWageDeduction(int id, [FromBody] WageDeductionDTO wageDeductionDTO)
        {
            try
            {
                var wageDeduction = await _context.WageDeductions.FindAsync(id);

                if (wageDeduction == null)
                {
                    return NotFound();
                }

                wageDeduction.FkDeductionsId = wageDeductionDTO.FkDeductionsId;
                wageDeduction.FkWageId = wageDeductionDTO.FkWageId;

                _context.Entry(wageDeduction).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WageDeductionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<WageDeductionDTO>> PostWageDeduction([FromBody] WageDeductionDTO wageDeductionDTO)
        {
            try
            {
                var wageDeduction = new WageDeduction
                {
                    FkDeductionsId = wageDeductionDTO.FkDeductionsId,
                    FkWageId = wageDeductionDTO.FkWageId,
                };

                _context.WageDeductions.Add(wageDeduction);
                _context.SaveChanges();
            }

            catch (DbUpdateException)

            {
                if (WageDeductionExists(wageDeductionDTO.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetWageDeduction", new { id = wageDeductionDTO.Id }, wageDeductionDTO);
        }

        // DELETE: api/WageDeductions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWageDeduction(int id)
        {
            var wageDeduction = await _context.WageDeductions.FindAsync(id);
            if (wageDeduction == null)
            {
                return NotFound();
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

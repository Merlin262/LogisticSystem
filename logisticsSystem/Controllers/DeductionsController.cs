using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using logisticsSystem.Data;
using logisticsSystem.Models;

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

        // GET: api/Deductions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Deduction>>> GetDeductions()
        {
            return await _context.Deductions.ToListAsync();
        }

        // GET: api/Deductions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Deduction>> GetDeduction(int id)
        {
            var deduction = await _context.Deductions.FindAsync(id);

            if (deduction == null)
            {
                return NotFound();
            }

            return deduction;
        }

        // PUT: api/Deductions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeduction(int id, Deduction deduction)
        {
            if (id != deduction.Id)
            {
                return BadRequest();
            }

            _context.Entry(deduction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeductionExists(id))
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

        // POST: api/Deductions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Deduction>> PostDeduction(Deduction deduction)
        {
            _context.Deductions.Add(deduction);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeductionExists(deduction.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDeduction", new { id = deduction.Id }, deduction);
        }

        // DELETE: api/Deductions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeduction(int id)
        {
            var deduction = await _context.Deductions.FindAsync(id);
            if (deduction == null)
            {
                return NotFound();
            }

            _context.Deductions.Remove(deduction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeductionExists(int id)
        {
            return _context.Deductions.Any(e => e.Id == id);
        }
    }
}

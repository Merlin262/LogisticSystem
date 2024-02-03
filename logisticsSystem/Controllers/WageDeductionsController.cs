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
        public async Task<IActionResult> PutWageDeduction(int id, WageDeduction wageDeduction)
        {
            if (id != wageDeduction.Id)
            {
                return BadRequest();
            }

            _context.Entry(wageDeduction).State = EntityState.Modified;

            try
            {
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

        // POST: api/WageDeductions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WageDeduction>> PostWageDeduction(WageDeduction wageDeduction)
        {
            _context.WageDeductions.Add(wageDeduction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWageDeduction", new { id = wageDeduction.Id }, wageDeduction);
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

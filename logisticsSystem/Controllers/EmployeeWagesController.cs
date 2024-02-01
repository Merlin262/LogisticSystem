using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using logisticsSystem.Models;
using logisticsSystem.Data;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeWagesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public EmployeeWagesController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeWages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeWage>>> GetEmployeeWages()
        {
            return await _context.EmployeeWages.ToListAsync();
        }

        // GET: api/EmployeeWages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeWage>> GetEmployeeWage(int id)
        {
            var employeeWage = await _context.EmployeeWages.FindAsync(id);

            if (employeeWage == null)
            {
                return NotFound();
            }

            return employeeWage;
        }

        // PUT: api/EmployeeWages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeWage(int id, EmployeeWage employeeWage)
        {
            if (id != employeeWage.Id)
            {
                return BadRequest();
            }

            _context.Entry(employeeWage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeWageExists(id))
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

        // POST: api/EmployeeWages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeWage>> PostEmployeeWage(EmployeeWage employeeWage)
        {
            _context.EmployeeWages.Add(employeeWage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeWageExists(employeeWage.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployeeWage", new { id = employeeWage.Id }, employeeWage);
        }

        // DELETE: api/EmployeeWages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeWage(int id)
        {
            var employeeWage = await _context.EmployeeWages.FindAsync(id);
            if (employeeWage == null)
            {
                return NotFound();
            }

            _context.EmployeeWages.Remove(employeeWage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeWageExists(int id)
        {
            return _context.EmployeeWages.Any(e => e.Id == id);
        }
    }
}

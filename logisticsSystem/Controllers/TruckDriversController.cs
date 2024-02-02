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
    public class TruckDriversController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public TruckDriversController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/TruckDrivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TruckDriver>>> GetTruckDrivers()
        {
            return await _context.TruckDrivers.ToListAsync();
        }

        // GET: api/TruckDrivers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TruckDriver>> GetTruckDriver(int id)
        {
            var truckDriver = await _context.TruckDrivers.FindAsync(id);

            if (truckDriver == null)
            {
                return NotFound();
            }

            return truckDriver;
        }

        // PUT: api/TruckDrivers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTruckDriver(int id, TruckDriver truckDriver)
        {
            if (id != truckDriver.Id)
            {
                return BadRequest();
            }

            _context.Entry(truckDriver).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TruckDriverExists(id))
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

        // POST: api/TruckDrivers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TruckDriver>> PostTruckDriver(TruckDriver truckDriver)
        {
            _context.TruckDrivers.Add(truckDriver);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTruckDriver", new { id = truckDriver.Id }, truckDriver);
        }

        // DELETE: api/TruckDrivers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruckDriver(int id)
        {
            var truckDriver = await _context.TruckDrivers.FindAsync(id);
            if (truckDriver == null)
            {
                return NotFound();
            }

            _context.TruckDrivers.Remove(truckDriver);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TruckDriverExists(int id)
        {
            return _context.TruckDrivers.Any(e => e.Id == id);
        }
    }
}

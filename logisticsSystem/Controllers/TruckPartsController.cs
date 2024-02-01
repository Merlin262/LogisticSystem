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
    public class TruckPartsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public TruckPartsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/TruckParts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TruckPart>>> GetTruckParts()
        {
            return await _context.TruckParts.ToListAsync();
        }

        // GET: api/TruckParts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TruckPart>> GetTruckPart(int id)
        {
            var truckPart = await _context.TruckParts.FindAsync(id);

            if (truckPart == null)
            {
                return NotFound();
            }

            return truckPart;
        }

        // PUT: api/TruckParts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTruckPart(int id, TruckPart truckPart)
        {
            if (id != truckPart.Id)
            {
                return BadRequest();
            }

            _context.Entry(truckPart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TruckPartExists(id))
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

        // POST: api/TruckParts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TruckPart>> PostTruckPart(TruckPart truckPart)
        {
            _context.TruckParts.Add(truckPart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TruckPartExists(truckPart.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTruckPart", new { id = truckPart.Id }, truckPart);
        }

        // DELETE: api/TruckParts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruckPart(int id)
        {
            var truckPart = await _context.TruckParts.FindAsync(id);
            if (truckPart == null)
            {
                return NotFound();
            }

            _context.TruckParts.Remove(truckPart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TruckPartExists(int id)
        {
            return _context.TruckParts.Any(e => e.Id == id);
        }
    }
}

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
    public class MaitenanceTruckPartsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public MaitenanceTruckPartsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/MaitenanceTruckParts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaitenanceTruckPart>>> GetMaitenanceTruckParts()
        {
            return await _context.MaitenanceTruckParts.ToListAsync();
        }

        // GET: api/MaitenanceTruckParts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaitenanceTruckPart>> GetMaitenanceTruckPart(int id)
        {
            var maitenanceTruckPart = await _context.MaitenanceTruckParts.FindAsync(id);

            if (maitenanceTruckPart == null)
            {
                return NotFound();
            }

            return maitenanceTruckPart;
        }

        // PUT: api/MaitenanceTruckParts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaitenanceTruckPart(int id, MaitenanceTruckPart maitenanceTruckPart)
        {
            if (id != maitenanceTruckPart.Id)
            {
                return BadRequest();
            }

            _context.Entry(maitenanceTruckPart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaitenanceTruckPartExists(id))
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

        // POST: api/MaitenanceTruckParts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaitenanceTruckPart>> PostMaitenanceTruckPart(MaitenanceTruckPart maitenanceTruckPart)
        {
            _context.MaitenanceTruckParts.Add(maitenanceTruckPart);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMaitenanceTruckPart", new { id = maitenanceTruckPart.Id }, maitenanceTruckPart);
        }

        // DELETE: api/MaitenanceTruckParts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaitenanceTruckPart(int id)
        {
            var maitenanceTruckPart = await _context.MaitenanceTruckParts.FindAsync(id);
            if (maitenanceTruckPart == null)
            {
                return NotFound();
            }

            _context.MaitenanceTruckParts.Remove(maitenanceTruckPart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaitenanceTruckPartExists(int id)
        {
            return _context.MaitenanceTruckParts.Any(e => e.Id == id);
        }
    }
}

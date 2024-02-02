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
        public async Task<IActionResult> PutAddress(int id, [FromBody] AddressDTO addressDTO)
        {
            try
            {
                var address = await _context.Addresses.FindAsync(id);

                if (address == null)
                {
                    return NotFound();
                }

                address.Country = addressDTO.Country;
                address.State = addressDTO.State;
                address.City = addressDTO.City;
                address.Street = addressDTO.Street;
                address.Number = addressDTO.Number;
                address.ComplementoComplement = addressDTO.ComplementoComplement;
                address.Zipcode = addressDTO.Zipcode;

                _context.Entry(address).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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
        public async Task<ActionResult<AddressDTO>> PostAddress([FromBody] AddressDTO addressDTO)
        {
            try
            {
                var address = new Address
                {
                    Id = addressDTO.Id,
                    Country = addressDTO.Country,
                    State = addressDTO.State,
                    City = addressDTO.City,
                    Street = addressDTO.Street,
                    Number = addressDTO.Number,
                    ComplementoComplement = addressDTO.ComplementoComplement,
                    Zipcode = addressDTO.Zipcode,
                };

                _context.Addresses.Add(address);
                _context.SaveChanges();
            }

            catch (DbUpdateException)

            {
                if (AddressExists(addressDTO.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAddress", new { id = addressDTO.Id }, addressDTO);
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

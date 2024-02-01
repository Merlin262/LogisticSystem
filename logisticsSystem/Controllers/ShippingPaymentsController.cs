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
    public class ShippingPaymentsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public ShippingPaymentsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/ShippingPayments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingPayment>>> GetShippingPayments()
        {
            return await _context.ShippingPayments.ToListAsync();
        }

        // GET: api/ShippingPayments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingPayment>> GetShippingPayment(int id)
        {
            var shippingPayment = await _context.ShippingPayments.FindAsync(id);

            if (shippingPayment == null)
            {
                return NotFound();
            }

            return shippingPayment;
        }

        // PUT: api/ShippingPayments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingPayment(int id, ShippingPayment shippingPayment)
        {
            if (id != shippingPayment.Id)
            {
                return BadRequest();
            }

            _context.Entry(shippingPayment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingPaymentExists(id))
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

        // POST: api/ShippingPayments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShippingPayment>> PostShippingPayment(ShippingPayment shippingPayment)
        {
            _context.ShippingPayments.Add(shippingPayment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ShippingPaymentExists(shippingPayment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetShippingPayment", new { id = shippingPayment.Id }, shippingPayment);
        }

        // DELETE: api/ShippingPayments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingPayment(int id)
        {
            var shippingPayment = await _context.ShippingPayments.FindAsync(id);
            if (shippingPayment == null)
            {
                return NotFound();
            }

            _context.ShippingPayments.Remove(shippingPayment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShippingPaymentExists(int id)
        {
            return _context.ShippingPayments.Any(e => e.Id == id);
        }
    }
}

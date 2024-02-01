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
    public class ItensStocksController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public ItensStocksController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/ItensStocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItensStock>>> GetItensStocks()
        {
            return await _context.ItensStocks.ToListAsync();
        }

        // GET: api/ItensStocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItensStock>> GetItensStock(int id)
        {
            var itensStock = await _context.ItensStocks.FindAsync(id);

            if (itensStock == null)
            {
                return NotFound();
            }

            return itensStock;
        }

        // PUT: api/ItensStocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItensStock(int id, ItensStock itensStock)
        {
            if (id != itensStock.Id)
            {
                return BadRequest();
            }

            _context.Entry(itensStock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItensStockExists(id))
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

        // POST: api/ItensStocks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItensStock>> PostItensStock(ItensStock itensStock)
        {
            _context.ItensStocks.Add(itensStock);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ItensStockExists(itensStock.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetItensStock", new { id = itensStock.Id }, itensStock);
        }

        // DELETE: api/ItensStocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItensStock(int id)
        {
            var itensStock = await _context.ItensStocks.FindAsync(id);
            if (itensStock == null)
            {
                return NotFound();
            }

            _context.ItensStocks.Remove(itensStock);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItensStockExists(int id)
        {
            return _context.ItensStocks.Any(e => e.Id == id);
        }
    }
}

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
    public class ItensShippedsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public ItensShippedsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/ItensShippeds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItensShippedDTO>>> GetItensShippeds()
        {
            try
            {
                var itensShippeds = await _context.ItensShippeds.ToListAsync();

                var itensShippedDTOs = itensShippeds.Select(itensShipped => new ItensShippedDTO
                {
                    FkItensStockId = itensShipped.FkItensStockId,
                    FkShippingId = itensShipped.FkShippingId,
                    // Mapeie outras propriedades conforme necessário
                });

                return Ok(itensShippedDTOs);
            }
            catch (Exception ex)
            {
                // Trate os erros adequadamente e retorne uma resposta apropriada
                return StatusCode(500, "Erro interno do servidor");
            }
        }


        // GET: api/ItensShippeds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItensShippedDTO>> GetItensShipped(int id)
        {
            try
            {
                var itensShipped = await _context.ItensShippeds.FindAsync(id);

                if (itensShipped == null)
                {
                    return NotFound();
                }

                var itensShippedDTO = new ItensShippedDTO
                {
                    FkItensStockId = itensShipped.FkItensStockId,
                    FkShippingId = itensShipped.FkShippingId,
                    // Mapeie outras propriedades conforme necessário
                };

                return Ok(itensShippedDTO);
            }
            catch (Exception ex)
            {
                // Trate os erros adequadamente e retorne uma resposta apropriada
                return StatusCode(500, "Erro interno do servidor");
            }
        }


        // PUT: api/ItensShippeds/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItensShipped(int id, ItensShippedDTO itensShippedDTO)
        {
            if (id != itensShippedDTO.FkItensStockId)
            {
                return BadRequest();
            }

            var itensShipped = new ItensShipped
            {
                FkItensStockId = itensShippedDTO.FkItensStockId,
                FkShippingId = itensShippedDTO.FkShippingId,
            };

            _context.Entry(itensShipped).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItensShippedExists(id))
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


        // POST: api/ItensShippeds
        [HttpPost]
        public async Task<ActionResult<ItensShippedDTO>> PostItensShipped([FromBody]ItensShippedDTO itensShippedDTO)
        {
            var itensShipped = new ItensShipped
            {
                FkItensStockId = itensShippedDTO.FkItensStockId,
                FkShippingId = itensShippedDTO.FkShippingId,
            };

            _context.ItensShippeds.Add(itensShipped);
            await _context.SaveChangesAsync();

            var createdDTO = new ItensShippedDTO
            {
                FkItensStockId = itensShipped.FkItensStockId,
                FkShippingId = itensShipped.FkShippingId,
            };

            return CreatedAtAction("GetItensShipped", new { id = itensShipped.FkItensStockId }, createdDTO);
        }


        // DELETE: api/ItensShippeds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItensShipped(int id)
        {
            var itensShipped = await _context.ItensShippeds.FindAsync(id);
            if (itensShipped == null)
            {
                return NotFound();
            }

            _context.ItensShippeds.Remove(itensShipped);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItensShippedExists(int id)
        {
            return _context.ItensShippeds.Any(e => e.Id == id);
        }
    }
}

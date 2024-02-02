using logisticsSystem.Data;

namespace logisticsSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using global::logisticsSystem.DTOs;
    using global::logisticsSystem.Models;

    namespace logisticsSystem.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class TrucksController : ControllerBase
        {
            private readonly LogisticsSystemContext _context;

            public TrucksController(LogisticsSystemContext context)
            {
                _context = context;
            }

            // GET: api/Trucks
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Truck>>> GetTrucks()
            {
                return await _context.Trucks.ToListAsync();
            }

            // GET: api/Trucks/5
            [HttpGet("{chassis}")]
            public async Task<ActionResult<Truck>> GetTruck(int chassis)
            {
                var truck = await _context.Trucks.FindAsync(chassis);

                if (truck == null)
                {
                    return NotFound();
                }

                return truck;
            }

            // PUT: api/Trucks/5
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPut("{chassis}")]
            public async Task<IActionResult> PutTruck(int chassis, [FromBody] TruckDTO truckDTO)
            {
                try
                {
                    var truck = await _context.Trucks.FindAsync(chassis);

                    if (truck == null)
                    {
                        return NotFound();
                    }

                    truck.Chassis = truckDTO.Chassis;
                    truck.MaximumWeight = truckDTO.MaximumWeight;
                    truck.KilometerCount = truckDTO.KilometerCount;
                    truck.Model = truckDTO.Model;
                    truck.Year = truckDTO.Year;
                    truck.Color = truckDTO.Color;

                    _context.Entry(truck).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TruckExists(chassis))
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
            public async Task<ActionResult<TruckDTO>> PostTruck([FromBody] TruckDTO truckDTO)
            {
                try
                {
                    var truck = new Truck
                    {
                        Chassis = truckDTO.Chassis,
                        MaximumWeight = truckDTO.MaximumWeight,
                        KilometerCount = truckDTO.KilometerCount,
                        Model = truckDTO.Model,
                        Year = truckDTO.Year,
                        Color = truckDTO.Color,
                    };

                    _context.Trucks.Add(truck);
                    _context.SaveChanges();
                }

                catch (DbUpdateException)

                {
                    if (TruckExists(truckDTO.Chassis))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetTruck", new { Chassis = truckDTO.Chassis }, truckDTO);
            }

            // DELETE: api/Trucks/5
            [HttpDelete("{chassis}")]
            public async Task<IActionResult> DeleteTruck(int chassis)
            {
                var truck = await _context.Trucks.FindAsync(chassis);
                if (truck == null)
                {
                    return NotFound();
                }

                _context.Trucks.Remove(truck);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool TruckExists(int chassis)
            {
                return _context.Trucks.Any(e => e.Chassis == chassis);
            }
        }
    }
}

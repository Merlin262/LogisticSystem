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
    public class TruckDriversController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public TruckDriversController(LogisticsSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTruckDrivers()
        {
            var truckDrivers = _context.TruckDrivers.ToList();
            var truckDriverDTOs = truckDrivers.Select(td => new TruckDriverDTO
            {
                FkTruckChassis = td.FkTruckChassis,
                FkEmployeeId = td.FkEmployeeId,
                
                
            });

            return Ok(truckDriverDTOs);
        }

        // GET: api/truck-drivers/{id}
        [HttpGet("{id}")]
        public IActionResult GetTruckDriverById(int id)
        {
            var truckDriver = _context.TruckDrivers.FirstOrDefault(td => td.Id == id);

            if (truckDriver == null)
            {
                return NotFound("Motorista de caminhão não encontrado.");
            }

            var truckDriverDTO = new TruckDriverDTO
            {
                FkTruckChassis = truckDriver.FkTruckChassis,
                FkEmployeeId = truckDriver.FkEmployeeId,
            };

            return Ok(truckDriverDTO);
        }

        // POST: api/truck-drivers
        [HttpPost]
        public IActionResult CreateTruckDriver([FromBody] TruckDriverDTO truckDriverDTO)
        {
            var truckDriver = new TruckDriver
            {
                FkTruckChassis = truckDriverDTO.FkTruckChassis,
                FkEmployeeId = truckDriverDTO.FkEmployeeId
            };

            _context.TruckDrivers.Add(truckDriver);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTruckDriverById), new { id = truckDriver.Id }, truckDriverDTO);
        }

        // PUT: api/truck-drivers/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTruckDriver(int id, [FromBody] TruckDriverDTO updatedTruckDriverDTO)
        {
            var existingTruckDriver = _context.TruckDrivers.FirstOrDefault(td => td.Id == id);

            if (existingTruckDriver == null)
            {
                return NotFound("Motorista de caminhão não encontrado.");
            }

            existingTruckDriver.FkTruckChassis = updatedTruckDriverDTO.FkTruckChassis;
            existingTruckDriver.FkEmployeeId = updatedTruckDriverDTO.FkEmployeeId;

            _context.SaveChanges();

            var updatedTruckDriverResponse = new TruckDriverDTO
            {
                FkTruckChassis = existingTruckDriver.FkTruckChassis,
                FkEmployeeId = existingTruckDriver.FkEmployeeId,
            };

            return Ok(updatedTruckDriverResponse);
        }

        // DELETE: api/truck-drivers/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTruckDriver(int id)
        {
            var truckDriver = _context.TruckDrivers.FirstOrDefault(td => td.Id == id);

            if (truckDriver == null)
            {
                return NotFound("Motorista de caminhão não encontrado.");
            }

            _context.TruckDrivers.Remove(truckDriver);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

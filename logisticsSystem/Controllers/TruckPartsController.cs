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
using logisticsSystem.Exceptions;

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

        // GET: api/truckparts
        [HttpGet]
        public IActionResult GetTruckParts()
        {
            var truckParts = _context.TruckParts.ToList();
            var truckPartDTOs = truckParts.Select(tp => new TruckPartDTO
            {
                Id = tp.Id,
                Description = tp.Description
            });

            return Ok(truckPartDTOs);
        }

        [HttpGet("{id}")]
        public IActionResult GetTruckPartById(int id)
        {
            var truckPart = _context.TruckParts.FirstOrDefault(tp => tp.Id == id);

            if (truckPart == null)
            {
                throw new NotFoundException("Peça de caminhão não encontrada.");
            }

            var truckPartDTO = new TruckPartDTO
            {
                Id = truckPart.Id,
                Description = truckPart.Description
            };

            return Ok(truckPartDTO);
        }


        [HttpPost]
        public IActionResult CreateTruckPart([FromBody] TruckPartDTO truckPartDTO)
        {
            if (truckPartDTO == null)
            {
                throw new NullRequestException("Dados inválidos para a peça de caminhão.");
            }

            var truckPart = new TruckPart
            {
                Description = truckPartDTO.Description
            };

            _context.TruckParts.Add(truckPart);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTruckPartById), new { id = truckPart.Id }, truckPartDTO);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateTruckPart(int id, [FromBody] TruckPartDTO truckPartDTO)
        {
            var truckPart = _context.TruckParts.FirstOrDefault(tp => tp.Id == id);

            if (truckPart == null)
            {
                throw new NotFoundException("Peça de caminhão não encontrada.");
            }

            if (truckPartDTO == null)
            {
                throw new NullRequestException("Dados inválidos para a peça de caminhão.");
            }

            truckPart.Description = truckPartDTO.Description;

            _context.SaveChanges();

            return Ok(truckPartDTO);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTruckPart(int id)
        {
            var truckPart = _context.TruckParts.FirstOrDefault(tp => tp.Id == id);

            if (truckPart == null)
            {
                throw new NotFoundException("Peça de caminhão não encontrada.");
            }

            _context.TruckParts.Remove(truckPart);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

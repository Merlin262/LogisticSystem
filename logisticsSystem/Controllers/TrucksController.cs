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
    using global::logisticsSystem.Exceptions;

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

            [HttpGet]
            public IActionResult GetTrucks()
            {
                var trucks = _context.Trucks.ToList();

                if (trucks == null || !trucks.Any())
                {
                    throw new NotFoundException("Nenhum caminhão encontrado.");
                }

                var truckDTOs = trucks.Select(t => new TruckDTO
                {
                    Chassis = t.Chassis,
                    KilometerCount = t.KilometerCount,
                    Model = t.Model,
                    Year = t.Year,
                    Color = t.Color,
                    TruckAxles = t.TruckAxles
                });

                return Ok(truckDTOs);
            }


            [HttpGet("{chassis}")]
            public IActionResult GetTruckByChassis(int chassis)
            {
                var truck = _context.Trucks.FirstOrDefault(t => t.Chassis == chassis);

                if (truck == null)
                {
                    throw new NotFoundException($"Caminhão com chassi {chassis} não encontrado.");
                }

                var truckDTO = new TruckDTO
                {
                    Chassis = truck.Chassis,
                    KilometerCount = truck.KilometerCount,
                    Model = truck.Model,
                    Year = truck.Year,
                    Color = truck.Color,
                    TruckAxles = truck.TruckAxles
                };

                return Ok(truckDTO);
            }


            [HttpPut("{chassis}")]
            public async Task<IActionResult> PutTruck(int chassis, [FromBody] TruckDTO truckDTO)
            {
                var truck = await _context.Trucks.FindAsync(chassis);

                if (truck == null)
                {
                    throw new NotFoundException($"Caminhão com chassi {chassis} não encontrado.");
                }

                // Verificar se os atributos estão presentes e não nulos
                if (truckDTO.Chassis == 0 || string.IsNullOrWhiteSpace(truckDTO.Model) || truckDTO.Year == 0)
                {
                    throw new InvalidDataTypeException("Os atributos do caminhão são inválidos.");
                }

                // Adicione mais verificações conforme necessário para outros atributos

                truck.Chassis = truckDTO.Chassis;
                truck.TruckAxles = truckDTO.TruckAxles;
                truck.KilometerCount = truckDTO.KilometerCount;
                truck.Model = truckDTO.Model;
                truck.Year = truckDTO.Year;
                truck.Color = truckDTO.Color;

                _context.Entry(truck).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }


            [HttpPost]
            public async Task<ActionResult<TruckDTO>> PostTruck([FromBody] TruckDTO truckDTO)
            {
                // Verificar se os atributos estão presentes e não nulos
                if (truckDTO.Chassis == 0 || string.IsNullOrWhiteSpace(truckDTO.Model) || truckDTO.Year == 0)
                {
                    throw new InvalidDataTypeException("Os atributos do caminhão são inválidos.");
                }

                // Adicione mais verificações conforme necessário para outros atributos

                var truck = new Truck
                {
                    Chassis = truckDTO.Chassis,
                    TruckAxles = truckDTO.TruckAxles,
                    KilometerCount = truckDTO.KilometerCount,
                    Model = truckDTO.Model,
                    Year = truckDTO.Year,
                    Color = truckDTO.Color,
                };

                _context.Trucks.Add(truck);
                _context.SaveChanges();

                var createdTruckDTO = new TruckDTO
                {
                    Chassis = truck.Chassis,
                    TruckAxles = truck.TruckAxles,
                    KilometerCount = truck.KilometerCount,
                    Model = truck.Model,
                    Year = truck.Year,
                    Color = truck.Color,
                };

                return CreatedAtAction("GetTruck", new { Chassis = createdTruckDTO.Chassis }, createdTruckDTO);
            }


            [HttpDelete("{chassis}")]
            public async Task<IActionResult> DeleteTruck(int chassis)
            {
                // Verificar se o caminhão existe
                var truck = await _context.Trucks.FindAsync(chassis);
                if (truck == null)
                {
                    throw new NotFoundException("Caminhão não encontrado.");
                }

                // Adicione mais verificações conforme necessário antes de excluir

                // Remover o caminhão do contexto
                _context.Trucks.Remove(truck);

                // Salvar as alterações no banco de dados
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}


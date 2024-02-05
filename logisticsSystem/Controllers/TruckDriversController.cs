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

        [HttpGet("{id}")]
        public IActionResult GetTruckDriverById(int id)
        {
            var truckDriver = _context.TruckDrivers.FirstOrDefault(td => td.Id == id);

            if (truckDriver == null)
            {
                throw new NotFoundException("Motorista de caminhão não encontrado.");
            }

            var truckDriverDTO = new TruckDriverDTO
            {
                FkTruckChassis = truckDriver.FkTruckChassis,
                FkEmployeeId = truckDriver.FkEmployeeId,
            };

            return Ok(truckDriverDTO);
        }


        [HttpPost]
        public IActionResult CreateTruckDriver([FromBody] TruckDriverDTO truckDriverDTO)
        {
            if (truckDriverDTO == null)
            {
                throw new NullRequestException("Solicitação inválida para criação de motorista de caminhão.");
            }

            // Verificar se FkTruckChassis é válido
            var truckChassisExists = _context.Trucks.Any(tc => tc.Chassis == truckDriverDTO.FkTruckChassis);
            if (!truckChassisExists)
            {
                throw new NotFoundException("Chassi do caminhão associado ao motorista não encontrado.");
            }

            // Verificar se FkEmployeeId é válido
            var employeeExists = _context.Employees.Any(e => e.FkPersonId == truckDriverDTO.FkEmployeeId);
            if (!employeeExists)
            {
                throw new NotFoundException("Funcionário associado ao motorista não encontrado.");
            }

            var truckDriver = new TruckDriver
            {
                FkTruckChassis = truckDriverDTO.FkTruckChassis,
                FkEmployeeId = truckDriverDTO.FkEmployeeId
            };

            _context.TruckDrivers.Add(truckDriver);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTruckDriverById), new { id = truckDriver.Id }, truckDriverDTO);
        }



        [HttpPut("{id}")]
        public IActionResult UpdateTruckDriver(int id, [FromBody] TruckDriverDTO updatedTruckDriverDTO)
        {
            var existingTruckDriver = _context.TruckDrivers.FirstOrDefault(td => td.Id == id);

            if (existingTruckDriver == null)
            {
                throw new NotFoundException("Motorista de caminhão não encontrado.");
            }

            // Verificar se FkTruckChassis é válido
            var truckChassisExists = _context.Trucks.Any(tc => tc.Chassis == updatedTruckDriverDTO.FkTruckChassis);
            if (!truckChassisExists)
            {
                throw new NotFoundException("Chassi do caminhão associado ao motorista não encontrado.");
            }

            // Verificar se FkEmployeeId é válido
            var employeeExists = _context.Employees.Any(e => e.FkPersonId == updatedTruckDriverDTO.FkEmployeeId);
            if (!employeeExists)
            {
                throw new NotFoundException("Funcionário associado ao motorista não encontrado.");
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


        [HttpDelete("{id}")]
        public IActionResult DeleteTruckDriver(int id)
        {
            var truckDriver = _context.TruckDrivers.FirstOrDefault(td => td.Id == id);

            if (truckDriver == null)
            {
                throw new NotFoundException("Motorista de caminhão não encontrado.");
            }

            _context.TruckDrivers.Remove(truckDriver);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

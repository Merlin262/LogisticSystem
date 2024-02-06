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
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public EmployeesController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        
        // GET para todos os funcionários
        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            var employees = _context.Employees.ToList();

            var employeesDto = employees.Select(e => new EmployeeDTO
            {
                FkPersonId = e.FkPersonId,
                Position = e.Position,
            }).ToList();

            return Ok(employeesDto);
        }

        // GET para funcionário por FkPersonId
        [HttpGet("{fkPersonId}")]
        public IActionResult GetEmployeeByFkPersonId(int fkPersonId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.FkPersonId == fkPersonId);

            if (employee == null)
            {
                throw new NotFoundException($"Employee de {fkPersonId} não encontrado no banco");
            }

            var employeeDto = new EmployeeDTO
            {
                FkPersonId = employee.FkPersonId,
                Position = employee.Position,
            };

            return Ok(employeeDto);
        }



        [HttpPost]
        public IActionResult CreateEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            var existingPerson = _context.People.FirstOrDefault(p => p.Id == employeeDTO.FkPersonId);
            if (existingPerson == null)
            {
                throw new NotFoundException($"Person with id '{employeeDTO.FkPersonId}' not found.");
            }

            if (employeeDTO.Position.Length > 255)
            {
                throw new InvalidDataException("Position length cannot exceed 255 characters.");
            }

            var newEmployee = new Employee
            {
                FkPersonId = employeeDTO.FkPersonId,
                Position = employeeDTO.Position,
            };

            _context.Employees.Add(newEmployee);
            _context.SaveChanges();

            _logger.WriteLogData($"Employee id '{newEmployee.FkPersonId}' registered successfully.");

            return Ok(newEmployee);
        }



        [HttpPut("{fkPersonId}")]
        public IActionResult UpdateEmployee(int fkPersonId, [FromBody] EmployeeDTO employeeDTO)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.FkPersonId == fkPersonId);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found in the database.");
            }

            if (employeeDTO.Position.Length > 255)
            {
                throw new InvalidDataException("Position length cannot exceed 255 characters.");
            }

            employee.Position = employeeDTO.Position;

            _context.SaveChanges();
            _logger.WriteLogData($"Employee id {fkPersonId} updated successfully.");

            return Ok(employee);
        }



        [HttpDelete("{fkPersonId}")]
        public IActionResult DeleteEmployee(int fkPersonId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.FkPersonId == fkPersonId);

            if (employee == null)
            {
                throw new NotFoundException("Employee não encontrado no banco"); 
            }

            _context.Employees.Remove(employee);

            _context.SaveChanges();
            _logger.WriteLogData($"Employee id {fkPersonId} deleted successfully.");

            return NoContent(); 
        }
    }
}

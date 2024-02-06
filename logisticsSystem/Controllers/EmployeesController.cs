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

        // CREATE - Método POST
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            // Mapear EmployeeDTO para a entidade Employee
            var newEmployee = new Employee
            {
                FkPersonId = employeeDTO.FkPersonId,
                Position = employeeDTO.Position,
            };

            // Adicionar o novo funcionário ao contexto
            _context.Employees.Add(newEmployee);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Employee id '{newEmployee.FkPersonId}' registered successfully.");

            // Retornar o novo funcionário criado
            return Ok(newEmployee);
        }

        // READ - Método GET (Todos)
        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            // Obter todos os funcionários
            var employees = _context.Employees.ToList();

            // Mapear Employee para EmployeeDTO
            var employeesDto = employees.Select(e => new EmployeeDTO
            {
                FkPersonId = e.FkPersonId,
                Position = e.Position,
            }).ToList();

            return Ok(employeesDto);
        }

        // READ - Método GET por FkPersonId
        [HttpGet("{fkPersonId}")]
        public IActionResult GetEmployeeByFkPersonId(int fkPersonId)
        {
            // Obter o funcionário com o FkPersonId fornecido
            var employee = _context.Employees.FirstOrDefault(e => e.FkPersonId == fkPersonId);

            if (employee == null)
            {
                throw new NotFoundException("Employee não encontrado no banco");
            }

            // Mapear Employee para EmployeeDTO
            var employeeDto = new EmployeeDTO
            {
                FkPersonId = employee.FkPersonId,
                Position = employee.Position,
            };

            return Ok(employeeDto);
        }

        // UPDATE - Método PUT
        [HttpPut("{fkPersonId}")]
        public IActionResult UpdateEmployee(int fkPersonId, [FromBody] EmployeeDTO employeeDTO)
        {
            // Obter o funcionário com o FkPersonId fornecido
            var employee = _context.Employees.FirstOrDefault(e => e.FkPersonId == fkPersonId);

            if (employee == null)
            {
                throw new NotFoundException("Employee não encontrado no banco");
            }

            // Atualizar propriedades do funcionário
            employee.Position = employeeDTO.Position;
            //employee.Commission = employeeDTO.Commission;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Employee id {fkPersonId} updated successfully.");

            return Ok(employee);
        }

        // DELETE - Método DELETE
        [HttpDelete("{fkPersonId}")]
        public IActionResult DeleteEmployee(int fkPersonId)
        {
            // Obter o funcionário com o FkPersonId fornecido
            var employee = _context.Employees.FirstOrDefault(e => e.FkPersonId == fkPersonId);

            if (employee == null)
            {
                throw new NotFoundException("Employee não encontrado no banco"); 
            }

            // Remover o funcionário do contexto
            _context.Employees.Remove(employee);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Employee id {fkPersonId} deleted successfully.");

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

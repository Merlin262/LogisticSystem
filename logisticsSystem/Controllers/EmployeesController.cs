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
    public class EmployeesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public EmployeesController(LogisticsSystemContext context)
        {
            _context = context;
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
                Commission = employeeDTO.Commission
            };

            // Adicionar o novo funcionário ao contexto
            _context.Employees.Add(newEmployee);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

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
                Commission = e.Commission
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
                return NotFound(); // Retorna 404 Not Found se o funcionário não for encontrado
            }

            // Mapear Employee para EmployeeDTO
            var employeeDto = new EmployeeDTO
            {
                FkPersonId = employee.FkPersonId,
                Position = employee.Position,
                Commission = employee.Commission
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
                return NotFound(); // Retorna 404 Not Found se o funcionário não for encontrado
            }

            // Atualizar propriedades do funcionário
            employee.Position = employeeDTO.Position;
            employee.Commission = employeeDTO.Commission;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

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
                return NotFound(); // Retorna 404 Not Found se o funcionário não for encontrado
            }

            // Remover o funcionário do contexto
            _context.Employees.Remove(employee);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

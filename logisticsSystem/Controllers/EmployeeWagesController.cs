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
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeWagesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly EmployeeWageService _employeeWageService;

        public EmployeeWagesController(LogisticsSystemContext context, EmployeeWageService employeeWageService)
        {
            _context = context;
            _employeeWageService = employeeWageService;
        }

        [HttpGet]
        public IActionResult GetAllRecords()
        {
            // Obter todos os registros de EmployeeWage
            var employeeWages = _context.EmployeeWages.ToList();

            // Mapear EmployeeWage para objeto dinâmico
            var result = employeeWages.Select(ew => new
            {
                Id = ew.Id,
                PayDay = ew.PayDay,
                Amount = ew.Amount,
                FkEmployeeId = ew.FkEmployeeId
            }).ToList();

            return Ok(result);
        }

        // CREATE - Método POST para EmployeeWage
        [HttpPost("{fkPersonId}/employeewages")]
        public IActionResult CreateEmployeeWage(int fkPersonId, [FromBody] EmployeeWageDTO employeeWageDTO)
        {
            // Obter o funcionário com o FkPersonId fornecido
            var employee = _context.Employees.FirstOrDefault(e => e.FkPersonId == fkPersonId);

            if (employee == null)
            {
                return NotFound(); // Retorna 404 Not Found se o funcionário não for encontrado
            }

            // Mapear EmployeeWageDTO para a entidade EmployeeWage
            var newEmployeeWage = new EmployeeWage
            {
                Id = employeeWageDTO.Id,
                PayDay = employeeWageDTO.PayDay,
                Amount = employeeWageDTO.Amount,
                FkEmployeeId = employee.FkPersonId
            };

            // Adicionar a nova remuneração ao contexto
            _context.EmployeeWages.Add(newEmployeeWage);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar a nova remuneração criada
            return Ok(new
            {
                newEmployeeWage.Id,
                newEmployeeWage.PayDay,
                newEmployeeWage.Amount,
                newEmployeeWage.FkEmployeeId
            });
        }

        // READ - Método GET (Todos) para EmployeeWage
        [HttpGet("{fkPersonId}/employeewages")]
        public IActionResult GetEmployeeWages(int fkPersonId)
        {
            // Obter as remunerações do funcionário com o FkPersonId fornecido
            var employeeWages = _context.EmployeeWages
                .Where(ew => ew.FkEmployee.FkPersonId == fkPersonId)
                .ToList();

            // Mapear EmployeeWage para EmployeeWageDTO
            var employeeWagesDto = employeeWages.Select(ew => new
            {
                ew.Id,
                ew.PayDay,
                ew.Amount,
                ew.FkEmployeeId
            }).ToList();

            return Ok(employeeWagesDto);
        }

        // UPDATE - Método PUT para EmployeeWage
        [HttpPut("{fkPersonId}/employeewages/{employeeWageId}")]
        public IActionResult UpdateEmployeeWage(int fkPersonId, int employeeWageId, [FromBody] EmployeeWageDTO employeeWageDTO)
        {
            // Obter a remuneração do funcionário com o FkPersonId e Id fornecidos
            var employeeWage = _context.EmployeeWages
                .FirstOrDefault(ew => ew.FkEmployee.FkPersonId == fkPersonId && ew.Id == employeeWageId);

            if (employeeWage == null)
            {
                return NotFound(); // Retorna 404 Not Found se a remuneração não for encontrada
            }

            // Atualizar propriedades da remuneração
            employeeWage.PayDay = employeeWageDTO.PayDay;
            employeeWage.Amount = employeeWageDTO.Amount;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(new
            {
                employeeWage.Id,
                employeeWage.PayDay,
                employeeWage.Amount,
                employeeWage.FkEmployeeId
            });
        }

        // DELETE - Método DELETE para EmployeeWage
        [HttpDelete("{fkPersonId}/employeewages/{employeeWageId}")]
        public IActionResult DeleteEmployeeWage(int fkPersonId, int employeeWageId)
        {
            // Obter a remuneração do funcionário com o FkPersonId e Id fornecidos
            var employeeWage = _context.EmployeeWages
                .FirstOrDefault(ew => ew.FkEmployee.FkPersonId == fkPersonId && ew.Id == employeeWageId);

            if (employeeWage == null)
            {
                return NotFound(); // Retorna 404 Not Found se a remuneração não for encontrada
            }

            // Remover a remuneração do contexto
            _context.EmployeeWages.Remove(employeeWage);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }

        // READ - Método GET (Todos) para EmployeeWage
        [HttpGet(("netSalary/{EmployeeWageId}"))]
        public IActionResult GetEmployeeNetSalary(int EmployeeWageId)
        {
            var netSalary = _context.EmployeeWages
                .Where(ew => ew.Id == EmployeeWageId)
                .Join(
                    _context.WageDeductions,
                    ew => ew.Id,
                    wd => wd.FkWageId,
                    (ew, wd) => new { Wage = ew, Deduction = wd }
                )
                .Join(
                    _context.Deductions,
                    joined => joined.Deduction.FkDeductionsId,
                    d => d.Id,
                    (joined, d) => new { ew = joined.Wage, deduction = joined.Deduction, DeductionDetail = d }
                )
                .Select(
                    result => result.ew.Amount + result.ew.Commission - result.DeductionDetail.Amount
                )
                .FirstOrDefault();


            return Ok(netSalary);
        }
    }
}

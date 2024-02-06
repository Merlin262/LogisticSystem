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
using logisticsSystem.Exceptions;

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

        [HttpGet("employeewages")]
        public IActionResult GetEmployeeWages()
        {
            var employeeWages = _context.EmployeeWages
                .Select(ew => new EmployeeWageDTO
                {
                    Id = ew.Id,
                    PayDay = ew.PayDay,
                    Amount = ew.Amount,
                    FkEmployeeId = ew.FkEmployeeId,
                    ComissionPercentage = ew.ComissionPercentage,
                    Commission = ew.Commission
                })
                .ToList();

            return Ok(employeeWages);
        }


        [HttpPost("/employeewages")]
        public IActionResult CreateEmployeeWage([FromBody] EmployeeWageDTO employeeWageDTO)
        {
            
            // Verificar se o FkEmployeeId é válido
            var existingEmployee = _context.Employees.Find(employeeWageDTO.FkEmployeeId);
            if (existingEmployee == null)
            {
                throw new NotFoundException($"Funcionário com ID {employeeWageDTO.FkEmployeeId} não encontrado.");
            }

            // Mapear EmployeeWageDTO para a entidade EmployeeWage
            var newEmployeeWage = new EmployeeWage
            {
                Id = employeeWageDTO.Id,
                PayDay = employeeWageDTO.PayDay,
                Amount = employeeWageDTO.Amount,
                FkEmployeeId = employeeWageDTO.FkEmployeeId,
                ComissionPercentage = employeeWageDTO.ComissionPercentage,
                Commission = employeeWageDTO.Commission
            };

            // Adicionar a nova remuneração ao contexto
            _context.EmployeeWages.Add(newEmployeeWage);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar a nova remuneração criada
            return Ok(newEmployeeWage);
            
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
                ew.FkEmployeeId,
                ew.ComissionPercentage,
                ew.Commission
            }).ToList();

            return Ok(employeeWagesDto);
        }

        // UPDATE - Método PUT para EmployeeWage
        [HttpPut("/employeewages/{employeeWageId}")]
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
                throw new NotFoundException(
                    "Employeewage não encontrado no banco");
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

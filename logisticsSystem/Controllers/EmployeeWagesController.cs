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
        private readonly LoggerService _logger;
        private readonly ReceiptService _receiptService;

        public EmployeeWagesController(LogisticsSystemContext context, EmployeeWageService employeeWageService, LoggerService logger, ReceiptService receiptService)
        {
            _context = context;
            _employeeWageService = employeeWageService;
            _logger = logger;
            _receiptService = receiptService;
        }


        // GET geral para EmployeeWage
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


        // GET para EmployeeWage por employeeId
        [HttpGet("/employeewages{employeeId}")]
        public IActionResult GetEmployeeWages(int employeeId)
        {
            var employeeExists = _context.Employees.Any(e => e.FkPersonId == employeeId);
            if (!employeeExists)
            {
                throw new NotFoundException($"Nenhum funcionário encontrado com ID {employeeId}.");
            }

            var employeeWages = _context.EmployeeWages
                .Where(ew => ew.FkEmployeeId == employeeId)
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
            var existingEmployee = _context.Employees.Find(employeeWageDTO.FkEmployeeId);
            if (existingEmployee == null)
            {
                throw new NotFoundException($"Funcionário com ID {employeeWageDTO.FkEmployeeId} não encontrado.");
            }

            if (employeeWageDTO.PayDay == default)
            {
                throw new InvalidDataException("Data de pagamento inválida.");
            }

            if (employeeWageDTO.Amount <= 0)
            {
                throw new InvalidDataException("O valor do salário deve ser maior que zero.");
            }

            if (employeeWageDTO.ComissionPercentage <= 0 )
            {
                throw new InvalidDataException("A porcentagem de comissão deve ser maior do que zero.");
            }

            if (employeeWageDTO.Commission < 0)
            {
                throw new InvalidDataException("A comissão não pode ser menor que zero.");
            }

            var newEmployeeWage = new EmployeeWage
            {
                Id = employeeWageDTO.Id,
                PayDay = employeeWageDTO.PayDay,
                Amount = employeeWageDTO.Amount,
                FkEmployeeId = employeeWageDTO.FkEmployeeId,
                ComissionPercentage = employeeWageDTO.ComissionPercentage,
                Commission = employeeWageDTO.Commission
            };

            _context.EmployeeWages.Add(newEmployeeWage);

            _context.SaveChanges();
            _logger.WriteLogData($"Employee wage do funcionário com ID '{newEmployeeWage.FkEmployeeId}' registrado com sucesso.");

            return Ok(newEmployeeWage);
        }


        [HttpPut("/employeewages/{employeeWageId}")]
        public IActionResult UpdateEmployeeWage(int fkPersonId, int employeeWageId, [FromBody] EmployeeWageDTO employeeWageDTO)
        {
            var employeeWage = _context.EmployeeWages
                .FirstOrDefault(ew => ew.FkEmployee.FkPersonId == fkPersonId && ew.Id == employeeWageId);

            if (employeeWage == null)
            {
                throw new NotFoundException($"Salário do funcionário com ID {employeeWageId} não encontrado.");
            }

            if (employeeWageDTO.PayDay == default)
            {
                throw new InvalidDataException("Data de pagamento inválida.");
            }

            if (employeeWageDTO.Amount <= 0)
            {
                throw new InvalidDataException("O valor do salário deve ser maior que zero.");
            }

            if (employeeWageDTO.ComissionPercentage <= 0)
            {
                throw new InvalidDataException("A porcentagem de comissão deve ser maior do que zero.");
            }

            if (employeeWageDTO.Commission <= 0)
            {
                throw new InvalidDataException("A comissão não pode ser menor que zero.");
            }

            employeeWage.PayDay = employeeWageDTO.PayDay;
            employeeWage.Amount = employeeWageDTO.Amount;
            employeeWage.ComissionPercentage = employeeWageDTO.ComissionPercentage;
            employeeWage.Commission = employeeWageDTO.Commission;

            _context.SaveChanges();
            _logger.WriteLogData($"Salário do funcionário com ID {employeeWageId} atualizado com sucesso.");

            return Ok(new
            {
                employeeWage.PayDay,
                employeeWage.Amount,
                employeeWage.ComissionPercentage,
                employeeWage.Commission,
                employeeWage.FkEmployeeId
            });
        }


        [HttpDelete("{fkPersonId}/employeewages/{employeeWageId}")]
        public IActionResult DeleteEmployeeWage(int fkPersonId, int employeeWageId)
        {
            var employeeWage = _context.EmployeeWages
                .FirstOrDefault(ew => ew.FkEmployee.FkPersonId == fkPersonId && ew.Id == employeeWageId);

            if (employeeWage == null)
            {
                throw new NotFoundException("Employeewage não encontrado no banco");
            }

            _context.EmployeeWages.Remove(employeeWage);

            _context.SaveChanges();
            _logger.WriteLogData($"Employee wage id {employeeWageId} deleted successfully.");

            return NoContent(); 
        }

        // GET para salario liquido do funcionário por EmployeeId
        [HttpGet("netSalary/{EmployeeId}")]
        public IActionResult GetEmployeeNetSalary(int EmployeeId)
        {
            var employeeExists = _context.Employees.Any(e => e.FkPersonId == EmployeeId);
            if (!employeeExists)
            {
                throw new NotFoundException($"Nenhum funcionário encontrado com ID {EmployeeId}.");
            }

            var netSalary = _employeeWageService.GetEmployeeNetSalary(EmployeeId);

            return Ok(netSalary);
        }

        
        [HttpGet("generatereceipt/{EmployeeId}")]
        public IActionResult GenerateEmGenerateEmployeeReceipt(int EmployeeId)
        {
            var employeeExists = _context.Employees.Any(e => e.FkPersonId == EmployeeId);
            if (!employeeExists)
            {
                throw new NotFoundException($"Nenhum funcionário encontrado com ID {EmployeeId}.");
            }

            var netSalary = _employeeWageService.GetEmployeeNetSalary(EmployeeId);

            _receiptService.GenerateEmployeeReceipt(EmployeeId, netSalary);

            return Ok("Receipt generated successfully!");
        }
    }
}

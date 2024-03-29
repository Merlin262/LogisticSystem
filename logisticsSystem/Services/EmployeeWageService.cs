﻿using logisticsSystem.Data;
using Microsoft.EntityFrameworkCore;
using logisticsSystem.Controllers.logisticsSystem.Controllers;
using logisticsSystem.DTOs;
using logisticsSystem.Controllers;
using logisticsSystem.Services;
using logisticsSystem.Models;

namespace logisticsSystem.Services
{
    public class EmployeeWageService
    {
        private readonly LogisticsSystemContext _context;
        public EmployeeWageService(LogisticsSystemContext context)
        {
            _context = context;
        }

        // Recebe a Comissão do Funcionário por determinado Frete
        public decimal GetEmployeeComission(int ShippingId)
        {
            //Calcula a comissão do motorista proporcional ao valor do envio
            var employee = _context.Shippings
                .Where(s => s.Id == ShippingId)
                .Join(
                    _context.EmployeeWages,
                    s => s.FkEmployeeId, 
                    e => e.FkEmployeeId,
                    (s, e) => e.ComissionPercentage * s.ShippingPrice
                )
                .FirstOrDefault();

            return employee;
        }

        // Recebe o Salário Líquido do Funcionário
        public decimal GetEmployeeNetSalary(int employeeId)
        {
            /*
             *   Faz a intercessão entre todas as tabelas necessárias (EmployeeWages, WageDeductions e Deductions
             *   para calcular o salário líquido somando os atributos de Amount e Comission (EmployeeWages) e subtraindo
             *   os valores de todos os descontos (Deductions)
             */
            var netSalary = _context.Employees
                .Where(e => e.FkPersonId == employeeId)
                .Join(
                    _context.EmployeeWages,
                    e => e.FkPersonId,
                    ew => ew.FkEmployeeId,
                    (e, ew) => new { Employee = e, EmployeeWage = ew }
                )
                .Join(
                    _context.WageDeductions,
                    joined => joined.EmployeeWage.Id,
                    wd => wd.FkWageId,
                    (joined, wd) => new { Employee = joined.Employee, EmployeeWage = joined.EmployeeWage, Deduction = wd }
                )
                .Join(
                    _context.Deductions,
                    joined => joined.Deduction.FkDeductionsId,
                    d => d.Id,
                    (joined, d) => new { Employee = joined.Employee, EmployeeWage = joined.EmployeeWage, Deduction = joined.Deduction, DeductionDetail = d }
                )
                .GroupBy(joined => new { joined.EmployeeWage.Id, joined.EmployeeWage.Amount, joined.EmployeeWage.Commission })
                .Select(group => new
                {
                    EmployeeId = group.Key.Id,
                    GrossSalary = group.Key.Amount + group.Key.Commission,
                    DeductionsTotal = group.Sum(item => item.DeductionDetail.Amount)
                })
                .Select(result => result.GrossSalary - result.DeductionsTotal)
                .FirstOrDefault();
              
            return netSalary;
        }
    }
}

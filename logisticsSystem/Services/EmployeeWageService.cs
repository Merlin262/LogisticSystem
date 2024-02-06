using logisticsSystem.Data;
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

        public decimal GetEmployeeComission(int ShippingId)
        {
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

        public decimal GetEmployeeNetSalary(int employeeId)
        {
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

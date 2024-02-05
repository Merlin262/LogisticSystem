using logisticsSystem.Data;
using Microsoft.EntityFrameworkCore;
using logisticsSystem.Controllers.logisticsSystem.Controllers;
using logisticsSystem.Data;
using logisticsSystem.DTOs;
using logisticsSystem.Controllers;
using logisticsSystem.Models;
using logisticsSystem.Services;

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
    }
}

using logisticsSystem.Data;
using logisticsSystem.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Queries : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public Queries(LogisticsSystemContext context)
        {
            _context = context;
        }

        [HttpGet("trucks-in-maintenance")]
        public IActionResult GetTrucksInMaintenance()
        {
            var trucksInMaintenance = _context.Trucks
                .Where(t => t.InMaintenance)
                .Select(t => new TruckDTO
                {
                    Chassis = t.Chassis,
                    KilometerCount = t.KilometerCount,
                    Model = t.Model,
                    Year = t.Year,
                    Color = t.Color,
                    TruckAxles = t.TruckAxles,
                    InMaintenance = t.InMaintenance,
                    LastMaintenanceKilometers = t.LastMaintenanceKilometers
                })
                .ToList();

            return Ok(trucksInMaintenance);
        }


        [HttpGet("employee-total-commission/{employeeId}")]
        public IActionResult GetEmployeeTotalCommission(int employeeId)
        {
            var totalCommission = _context.EmployeeWages
                .Where(e => e.FkEmployeeId == employeeId)
                .GroupBy(e => e.FkEmployeeId)
                .Select(g => new { FkEmployeeId = g.Key, TotalCommission = g.Sum(e => e.Commission) })
                .FirstOrDefault();

            return Ok(totalCommission);
        }

        [HttpGet("order-items-by-client/{clientId}")]
        public IActionResult GetOrderItemsByClient(int clientId)
        {
            // Verificar se o cliente existe
            var clientExists = _context.Clients.Any(c => c.FkPersonId == clientId);
            if (!clientExists)
            {
                return NotFound("Cliente não encontrado.");
            }

            // Buscar os itens dos pedidos associados aos envios do cliente
            var orderItems = _context.Shippings
                .Where(s => s.FkClientId == clientId)
                .SelectMany(s => s.ItensShippeds)
                .Where(isp => isp.FkItensStock != null) // Adicione esta condição se necessário
                .Select(isp => new
                {
                    ItemName = isp.FkItensStock.Description,
                    Quantity = isp.QuantityItens
                })
                .ToList();

            return Ok(orderItems);
            
        }


        [HttpGet("available-stock-items")]
        public IActionResult GetAvailableStockItems()
        {
            var availableStockItems = _context.ItensStocks
                .Where(isp => isp.Quantity > 0)
                .Select(isp => new ItensStockDTO
                {
                    Description = isp.Description,
                    Quantity = isp.Quantity,
                    Price = isp.Price,
                    Weight = isp.Weight
                })
                .ToList();

            return Ok(availableStockItems);
        }


        [HttpGet("clients")]
        public IActionResult GetClients()
        {
            var clients = _context.Clients
                .Include(c => c.FkPerson)
                .Select(c => new QueriesDTO()
                {
                    name = c.FkPerson.Name,
                    email = c.FkPerson.Email,
                    BirthDate = c.FkPerson.BirthDate,
                    // Outras propriedades do DTO...
                })
                .ToList();

            return Ok(clients);
        }







    }
}

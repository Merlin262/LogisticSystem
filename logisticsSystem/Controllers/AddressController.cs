using logisticsSystem.Data;
using logisticsSystem.DTOs;
using logisticsSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace logisticsSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public AddressController(LogisticsSystemContext context)
        {
            _context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            try
            {
                var addresses = _context.Addresses;
                var addressDTOs = addresses.Select(address => new AddressDTO
                {
                    Country = address.Country,
                    State = address.State,
                    City = address.City,
                    Street = address.Street,
                    Number = address.Number,
                    ComplementoComplement = address.ComplementoComplement,
                    Zipcode = address.Zipcode,
                    Id = address.Id
                });
                return Ok(addressDTOs);
            }
            catch (DbUpdateException ex)
            {
                // Logar a exceção usando um sistema de logging (por exemplo, ILogger)
                // e fornecer uma mensagem de erro mais específica
                return StatusCode(500, "Erro de atualização no banco de dados");
            }
            catch (Exception ex)
            {
                // Logar a exceção e fornecer uma mensagem de erro genérica
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }


}

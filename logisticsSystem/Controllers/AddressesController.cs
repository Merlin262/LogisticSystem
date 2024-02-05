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
using Microsoft.Data.SqlClient;
using logisticsSystem.Exceptions;
using System.Text.RegularExpressions;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public AddressesController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        // GET: api/Addresses/5
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                throw new NotFoundException($"Endereço de id: {id} não encontrado no banco de dados ");
            }

            return address;
        }


        // POST: api/Addresses/create
        [HttpPost("create")]
        public async Task<ActionResult<AddressDTO>> PostAddress([FromBody] AddressDTO addressDTO)
        {
            // Validar se Country contém apenas letras
            if (!Regex.IsMatch(addressDTO.Country, "^[a-zA-Z]+$"))
            {
                throw new InvalidDataTypeException("Country deve conter apenas letras.");
            }

            // Validar se State contém apenas letras
            if (!Regex.IsMatch(addressDTO.State, "^[a-zA-Z]+$"))
            {
                throw new InvalidDataTypeException("State deve conter apenas letras.");
            }

            // Validar se Zipcode contém apenas números
            if (!Regex.IsMatch(addressDTO.Zipcode, "^[0-9]+$"))
            {
                throw new InvalidDataTypeException("Zipcode deve conter apenas números.");
            }

            var address = new Address
            {
                Country = addressDTO.Country,
                State = addressDTO.State,
                City = addressDTO.City,
                Street = addressDTO.Street,
                Number = addressDTO.Number,
                Complement = addressDTO.Complement,
                Zipcode = addressDTO.Zipcode,
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddress", new { id = address.Id }, addressDTO);
        }

        // PUT: api/Addresses/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutAddress(int id, [FromBody] AddressDTO addressDTO)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                throw new NotFoundException($"Endereço de id: {id} não encontrado no banco de dados ");
            }

            // Validar se Country contém apenas letras
            if (!Regex.IsMatch(addressDTO.Country, "^[a-zA-Z]+$"))
            {
                throw new InvalidDataTypeException("Country deve conter apenas letras.");
            }

            // Validar se State contém apenas letras
            if (!Regex.IsMatch(addressDTO.State, "^[a-zA-Z]+$"))
            {
                throw new InvalidDataTypeException("State deve conter apenas letras.");
            }

            // Validar se Zipcode contém apenas números
            if (!Regex.IsMatch(addressDTO.Zipcode, "^[0-9]+$"))
            {
                throw new InvalidDataTypeException("Zipcode deve conter apenas números.");
            }


            address.Country = addressDTO.Country;
            address.State = addressDTO.State;
            address.City = addressDTO.City;
            address.Street = addressDTO.Street;
            address.Number = addressDTO.Number;
            address.Complement = addressDTO.Complement;
            address.Zipcode = addressDTO.Zipcode;

            _context.Entry(address).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                throw new NotFoundException($"Endereço de id: {id} não encontrado no banco de dados ");
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

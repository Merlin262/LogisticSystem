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
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public AddressesController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET geral para todos os endereços
        [HttpGet("/addresses")]
        public IActionResult GetAddresses()
        {
            var addresses = _context.Addresses.ToList();

            var addressesDto = addresses.Select(a => new AddressDTO
            {
                Id = a.Id,
                Country = a.Country,
                State = a.State,
                City = a.City,
                Street = a.Street,
                Number = a.Number,
                Complement = a.Complement,
                Zipcode = a.Zipcode,
            }).ToList();

            return Ok(addressesDto);
        }


        // GET para endereço por ID
        [HttpGet("/addresses/{id}")]
        public IActionResult GetAddressById(int id)
        {
            var address = _context.Addresses.FirstOrDefault(a => a.Id == id);

            if (address == null)
            {
                throw new NotFoundException($"Endereço de {id} não encontrado.");
            }

            var addressDto = new AddressDTO
            {
                Id = address.Id,
                Country = address.Country,
                State = address.State,
                City = address.City,
                Street = address.Street,
                Number = address.Number,
                Complement = address.Complement,
                Zipcode = address.Zipcode,
            };

            return Ok(addressDto);
        }



        [HttpPost("create-address")]
        public async Task<ActionResult<AddressDTO>> PostAddress([FromBody] AddressDTO addressDTO)
        {
            // Validar se Country contém apenas letras e espaços
            if (!Regex.IsMatch(addressDTO.Country, @"^[a-zA-Z\s]+$"))
            {
                throw new InvalidDataTypeException("Country deve conter apenas letras e espaços.");
            }

            // Validar se State contém apenas letras e espaços
            if (!Regex.IsMatch(addressDTO.State, @"^[a-zA-Z\s]+$"))
            {
                throw new InvalidDataTypeException("State deve conter apenas letras e espaços.");
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
            _logger.WriteLogData($"Address de Id {address.Id} registrado com sucesso.");

            return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, addressDTO);
        }


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
            _logger.WriteLogData($"Address de Id: {id} atualizado com sucesso.");

            return NoContent();
        }


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
            _logger.WriteLogData($"Address de Id: {id} deletado com sucesso.");

            return NoContent();
        }
    }
}
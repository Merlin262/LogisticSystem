﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using logisticsSystem.Data;
using logisticsSystem.Models;
using logisticsSystem.DTOs;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, [FromBody] AddressDTO addressDTO)
        {
            try
            {
                var address = await _context.Addresses.FindAsync(id);

                if (address == null)
                {
                    return NotFound();
                }

                address.Country = addressDTO.Country;
                address.State = addressDTO.State;
                address.City = addressDTO.City;
                address.Street = addressDTO.Street;
                address.Number = addressDTO.Number;
                address.ComplementoComplement = addressDTO.ComplementoComplement;
                address.Zipcode = addressDTO.Zipcode;

                _context.Entry(address).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<AddressDTO>> PostAddress([FromBody] AddressDTO addressDTO)
        {
            try
            { 
                var address = new Address
                {
                    Id = addressDTO.Id,
                    Country = addressDTO.Country,
                    State = addressDTO.State,
                    City = addressDTO.City,
                    Street = addressDTO.Street,
                    Number = addressDTO.Number,
                    ComplementoComplement = addressDTO.ComplementoComplement,
                    Zipcode = addressDTO.Zipcode,
                };

                _context.Addresses.Add(address);
                _context.SaveChanges();
            }

            catch (DbUpdateException)
            
            {
                if (AddressExists(addressDTO.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAddress", new { id = addressDTO.Id }, addressDTO);
        }
    

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }
    }
}

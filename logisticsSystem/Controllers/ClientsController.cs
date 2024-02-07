using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using logisticsSystem.Data;
using logisticsSystem.DTOs;
using System.Text.Json.Serialization;
using System.Text.Json;
using logisticsSystem.Models;
using logisticsSystem.Exceptions;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net;
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public ClientsController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET geral para todos os clientes
        [HttpGet("/clients")]
        public IActionResult GetClients()
        {
            var clients = _context.Clients.ToList();

            var clientsDto = clients.Select(e => new ClientDTO
            {
                FkPersonId = e.FkPersonId,
            }).ToList();

            return Ok(clientsDto);
        }


        // GET para cliente por ID
        [HttpGet("/clients/{id}")]
        public IActionResult GetClientById(int id)
        {
            var client = _context.Clients.FirstOrDefault(c => c.FkPersonId == id);

            if (client == null)
            {
                throw new NotFoundException($"Client de ID: {id} não encontrado no banco de daods.");
            }

            var clientDto = new ClientDTO
            {
                FkPersonId = client.FkPersonId,
            };

            return Ok(clientDto);
        }


        [HttpPost("/clients")]
        public IActionResult CreateClient([FromBody] ClientDTO clientDTO)
        {
            var personExists = _context.People.Any(p => p.Id == clientDTO.FkPersonId);
            if (!personExists)
            {
                throw new NotFoundException($"Pessoa de ID {clientDTO.FkPersonId} não encontrado.");
            }

            var newClient = new Client
            {
                FkPersonId = clientDTO.FkPersonId
            };

            _context.Clients.Add(newClient);

            _context.SaveChanges();
            _logger.WriteLogData($"Client id {newClient.FkPersonId} registered succesfully.");

            return CreatedAtAction(nameof(GetClients), new { FkPersonId = newClient.FkPersonId }, clientDTO);
        }


        [HttpPut("/clients/{id}")]
        public IActionResult UpdateClient(int id, [FromBody] ClientDTO updatedClientDTO)
        {
            
            var existingClient = _context.Clients.FirstOrDefault(c => c.FkPersonId == id);

            if (existingClient == null)
            {
                throw new NotFoundException($"Client de ID {id} não encontrado no banco de daods.");
            }

            existingClient.FkPersonId = updatedClientDTO.FkPersonId;

            _context.SaveChanges();
            _logger.WriteLogData($"Client id {id} updated successfully.");

            return Ok(new
            {
                FkPersonId = existingClient.FkPersonId,
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                throw new NotFoundException($"Client de ID: {id} não encontrado no banco de daods.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            _logger.WriteLogData($"Client id {id} deleted successfully.");

            return NoContent();
        }
    }
}

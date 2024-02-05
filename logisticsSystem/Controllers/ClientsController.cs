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

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public ClientsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        [HttpGet("/clients")]
        public IActionResult GetClients()
        {
            // Obter todos os ClientDTOs do contexto
            var clients = _context.Clients.ToList();

            var clientsDto = clients.Select(e => new ClientDTO
            {
                FkPersonId = e.FkPersonId,
            }).ToList();

            // Retornar a lista de ClientDTOs
            return Ok(clientsDto);
        }

        [HttpGet("/clients/{id}")]
        public IActionResult GetClientById(int id)
        {
            // Obter o cliente pelo ID do contexto
            var client = _context.Clients.FirstOrDefault(c => c.FkPersonId == id);

            if (client == null)
            {
                throw new NotFoundException("Client não encontrado no banco de daods.");
            }

            // Mapear o cliente para um ClientDTO
            var clientDto = new ClientDTO
            {
                FkPersonId = client.FkPersonId,
            };

            // Retornar o ClientDTO específico pelo ID
            return Ok(clientDto);
        }

        [HttpPost("/clients")]
        public IActionResult CreateClient([FromBody] ClientDTO clientDTO)
        {
            // Validar se FkPersonId é maior que 0
            if (clientDTO.FkPersonId <= 0)
            {
                throw new InvalidDataException("FkPersonId deve ser maior que zero.");
            }

            // Mapear ClientDTO para a entidade Client
            var newClient = new Client
            {
                FkPersonId = clientDTO.FkPersonId
                // Adicione outras propriedades conforme necessário
            };

            // Adicionar o novo cliente ao contexto
            _context.Clients.Add(newClient);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o novo cliente criado
            return CreatedAtAction("GetClient", new { FkPersonId = newClient.FkPersonId }, clientDTO);
        }



        [HttpPut("/clients/{id}")]
        public IActionResult UpdateClient(int id, [FromBody] ClientDTO updatedClientDTO)
        {
            
            // Obter o cliente pelo ID do contexto
            var existingClient = _context.Clients.FirstOrDefault(c => c.FkPersonId == id);

            if (existingClient == null)
            {
                throw new NotFoundException("Client não encontrado no banco de daods.");
            }

            // Atualizar as propriedades do cliente existente com base no DTO fornecido
            existingClient.FkPersonId = updatedClientDTO.FkPersonId;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o cliente atualizado
            return Ok(new
            {
                FkPersonId = existingClient.FkPersonId,
            });
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                throw new NotFoundException("Client não encontrado no banco de daods.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

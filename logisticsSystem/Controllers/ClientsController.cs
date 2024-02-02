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
using System.Text.Json.Serialization;
using System.Text.Json;

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

        [HttpGet("api/clients")]
        public IActionResult GetAllClients()
        {
            // Obter todos os clientes
            var clients = _context.Clients
                .Include(c => c.FkPerson)
                .ThenInclude(p => p.FkAddress)
                .ToList();

            // Mapear os clientes para o formato desejado
            var clientsDto = clients.Select(client => new
            {
                Id = client.FkPerson.Id, // Ajuste aqui conforme necessário
                client.FkPerson.Name,
                client.FkPerson.Email,
                FkAddress = new
                {
                    client.FkPerson.FkAddress.Country,
                    client.FkPerson.FkAddress.State,
                    client.FkPerson.FkAddress.City,
                    client.FkPerson.FkAddress.Street,
                    client.FkPerson.FkAddress.Number,
                    client.FkPerson.FkAddress.Complement,
                    client.FkPerson.FkAddress.Zipcode,
                    Id = client.FkPerson.FkAddress.Id // Ajuste aqui conforme necessário
                }
            }).ToList();

            // Converta a lista para JSON e imprima no console
            var jsonResult = JsonSerializer.Serialize(clientsDto, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(jsonResult);

            return Ok(clientsDto); // Retorna 200 OK com os dados dos clientes no formato desejado
        }


        [HttpGet("api/clients/{id}")]
        public IActionResult GetClientById(int id)
        {
            // Obter o cliente com o ID fornecido
            var client = _context.Clients
                .Include(c => c.FkPerson)
                .ThenInclude(p => p.FkAddress)
                .FirstOrDefault(c => c.FkPerson.Id == id);

            if (client == null)
            {
                return NotFound(); // Retorna 404 Not Found se o cliente não for encontrado
            }

            // Mapear a entidade Client para o formato desejado
            var clientDto = new
            {
                Id = client.FkPerson.Id, // Ajuste aqui conforme necessário
                client.FkPerson.Name,
                client.FkPerson.Email,
                FkAddress = new
                {
                    client.FkPerson.FkAddress.Country,
                    client.FkPerson.FkAddress.State,
                    client.FkPerson.FkAddress.City,
                    client.FkPerson.FkAddress.Street,
                    client.FkPerson.FkAddress.Number,
                    client.FkPerson.FkAddress.Complement,
                    client.FkPerson.FkAddress.Zipcode,
                    Id = client.FkPerson.FkAddress.Id // Ajuste aqui conforme necessário
                }
            };

            // Converta o objeto para JSON e imprima no console
            var jsonResult = JsonSerializer.Serialize(clientDto, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(jsonResult);

            return Ok(clientDto); // Retorna 200 OK com os dados do cliente no formato desejado
        }



        [HttpPost("api/clients")]
        public IActionResult CreateClient([FromBody] ClientDTO request)
        {
            // Crie uma nova instância de Client e relacione-a com Person e Address conforme necessário
            var newClient = new Client
            {
                FkPerson = new Person
                {
                    Id = request.Id,
                    Name = request.Name,
                    Email = request.Email,
                    FkAddress = new Address
                    {
                        Id = request.FkAddress.Id,
                        Country = request.FkAddress.Country,
                        State = request.FkAddress.State,
                        City = request.FkAddress.City,
                        Street = request.FkAddress.Street,
                        Number = request.FkAddress.Number
                    }
                }
            };

            // Adicione o novo cliente ao contexto
            _context.Clients.Add(newClient);

            // Salve as alterações no banco de dados
            _context.SaveChanges();

            // Configure as opções para lidar com referências cíclicas
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 32 // Defina um valor adequado para a profundidade máxima, se necessário
            };

            // Converta o novo cliente para JSON
            var jsonResult = JsonSerializer.Serialize(newClient, jsonOptions);

            // Retorne o novo cliente criado
            return Ok(jsonResult);
        }




        [HttpPut("api/clients/{id}")]
        public IActionResult UpdateClient(int id, [FromBody] ClientDTO request)
        {
            // Obter o cliente com o ID fornecido
            var client = _context.Clients.Find(id);

            if (client == null)
            {
                return NotFound(); // Retorna 404 Not Found se o cliente não for encontrado
            }

            // Atualizar propriedades do cliente
            client.FkPerson.Name = request.Name;
            client.FkPerson.Email = request.Email;

            // Atualizar propriedades do endereço
            client.FkPerson.FkAddress.Country = request.FkAddress.Country;
            client.FkPerson.FkAddress.State = request.FkAddress.State;
            client.FkPerson.FkAddress.City = request.FkAddress.City;
            client.FkPerson.FkAddress.Street = request.FkAddress.Street;
            client.FkPerson.FkAddress.Number = request.FkAddress.Number;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(client); // Retorna 200 OK com os dados atualizados do cliente
        }





        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.FkPersonId == id);
        }

        
    }
}

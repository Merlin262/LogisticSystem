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
        private readonly HandleException _handleException;

        public ClientsController(LogisticsSystemContext context, HandleException handleException)
        {
            _context = context;
            _handleException = handleException;
        }

        [HttpGet("api/clients")]
        public IActionResult GetAllClients()
        {
            try
            {
                // Obter todos os clientes
                var clients = _context.Clients
                    .Include(c => c.FkPerson)
                    .ThenInclude(p => p.FkAddress)
                    .ToList();

                // Verificar se a lista de clientes está vazia
                if (clients.Count == 0)
                {
                    throw new NotFoundException("Nenhum cliente cadastrado no banco de dados.");
                }

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
            catch (SqlException ex) // Adicionei uma captura específica para SqlException
            {
                // Trate a exceção de falta de conexão com o banco de dados
                throw new DatabaseConnectionException("Erro de conexão com o banco de dados.");
            }
            catch (Exception ex)
            {
                // Outras exceções não tratadas
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw new InternalServerException("Erro interno do servidor.");
            }
        }



        [HttpGet("api/clients/{id}")]
        public IActionResult GetClientById(int id)
        {
            try
            {
                // Obter o cliente com o ID fornecido
                var client = _context.Clients
                    .Include(c => c.FkPerson)
                    .ThenInclude(p => p.FkAddress)
                    .FirstOrDefault(c => c.FkPerson.Id == id);

                if (client == null)
                {
                    throw new NotFoundException("Cliente não encontrado, verifique se o ID esta correto"); 
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
            catch (SqlException ex) // Adicionei uma captura específica para SqlException
            {
                // Trate a exceção de falta de conexão com o banco de dados
                Console.WriteLine($"Erro de conexão com o banco de dados: {ex.Message}");
                throw new DatabaseConnectionException("Erro de conexão com o banco de dados.");
            }
            catch (Exception ex)
            {
                // Outras exceções não tratadas
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw new InternalServerException("Erro interno do servidor.");
            }
        }



        [HttpPost("api/clients")]
        public IActionResult CreateClient([FromBody] ClientDTO request)
        {
            try
            {
                // Verifique se a solicitação é nula
                if (request == null)
                {
                    throw new NullRequestException("A solicitação é nula.");
                }

                // Verifique os tipos de dados esperados
                if (request.Id <= 0 || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email)
                    || request.FkAddress == null || request.FkAddress.Id <= 0
                    || string.IsNullOrEmpty(request.FkAddress.Country) || string.IsNullOrEmpty(request.FkAddress.State)
                    || string.IsNullOrEmpty(request.FkAddress.City) || string.IsNullOrEmpty(request.FkAddress.Street)
                    || string.IsNullOrEmpty(request.FkAddress.Number))
                {
                    throw new InvalidDataTypeException("Alguns campos têm tipos de dados inválidos ou estão ausentes.");
                }

                // Verifique se o nome contém apenas letras
                if (!IsAlpha(request.Name))
                {
                    throw new InvalidDataTypeException("O nome deve conter apenas letras.");
                }

                if (!IsAlpha(request.FkAddress.Country))
                {
                    throw new InvalidDataTypeException("O país deve conter apenas letras.");
                }

                if (!IsAlpha(request.FkAddress.State))
                {
                    throw new InvalidDataTypeException("O estado deve conter apenas letras.");
                }

                if (!IsAlpha(request.FkAddress.City))
                {
                    throw new InvalidDataTypeException("A cidade deve conter apenas letras.");
                }

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
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro de conexão com o banco de dados: {ex.Message}");
                throw new DatabaseConnectionException("Erro de conexão com o banco de dados.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro de serialização JSON: {ex.Message}");
                throw new InvalidDataTypeException("Erro na serialização JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw new InternalServerException("Erro interno do servidor.");
            }
        }


        [HttpPut("api/clients/{id}")]
        public IActionResult UpdateClient(int id, [FromBody] ClientDTO request)
        {
            try
            {
                // Obter o cliente com o ID fornecido
                var client = _context.Clients.Find(id);

                if (client == null)
                {
                    throw new NotFoundException("Cliente não encontrado para fazer a mudança"); // Retorna 404 Not Found se o cliente não for encontrado
                }

                // Verifique os tipos de dados esperados
                if (request.Id <= 0 || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email)
                    || request.FkAddress == null || request.FkAddress.Id <= 0
                    || string.IsNullOrEmpty(request.FkAddress.Country) || string.IsNullOrEmpty(request.FkAddress.State)
                    || string.IsNullOrEmpty(request.FkAddress.City) || string.IsNullOrEmpty(request.FkAddress.Street)
                    || string.IsNullOrEmpty(request.FkAddress.Number))
                {
                    throw new InvalidDataTypeException("Alguns campos têm tipos de dados inválidos ou estão ausentes.");
                }

                // Verifique se o nome contém apenas letras
                if (!IsAlpha(request.Name))
                {
                    throw new InvalidDataTypeException("O nome deve conter apenas letras.");
                }

                if (!IsAlpha(request.FkAddress.Country))
                {
                    throw new InvalidDataTypeException("O país deve conter apenas letras.");
                }

                if (!IsAlpha(request.FkAddress.State))
                {
                    throw new InvalidDataTypeException("O estado deve conter apenas letras.");
                }

                if (!IsAlpha(request.FkAddress.City))
                {
                    throw new InvalidDataTypeException("A cidade deve conter apenas letras.");
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
            catch (Exception ex)
            {
                _handleException.HandleExceptioGeneric(ex);
                throw; // Re-throw a exception para manter o comportamento original
            }
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


        private bool IsAlpha(string value)
        {
            // Use uma expressão regular para verificar se a string contém apenas letras
            return Regex.IsMatch(value, @"^[a-zA-Z]+$");
        }


    }
}

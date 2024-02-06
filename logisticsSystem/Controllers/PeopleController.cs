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
using logisticsSystem.Exceptions;
using System.Text.RegularExpressions;
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;

        public PeopleController(LogisticsSystemContext context, LoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreatePerson([FromBody] PersonDTO personDTO)
        {
            // Verificar se a solicitação é nula
            if (personDTO == null)
            {
                throw new NullRequestException("Solicitação inválida para criação de pessoa.");
            }

            // Validar propriedades da pessoa antes da criação
            if (string.IsNullOrWhiteSpace(personDTO.Name))
            {
                throw new InvalidDataException("O nome da pessoa não pode ser nulo ou vazio.");
            }

            // Validar se o campo Name contém apenas letras
            if (!Regex.IsMatch(personDTO.Name, "^[a-zA-Z]+$"))
            {
                throw new InvalidDataException("O nome da pessoa deve conter apenas letras.");
            }

            if (string.IsNullOrWhiteSpace(personDTO.Email))
            {
                throw new InvalidDataException("O e-mail da pessoa não pode ser nulo ou vazio.");
            }

            // Validar se o CPF é único
            if (_context.People.Any(p => p.CPF == personDTO.CPF))
            {
                throw new InvalidDataException($"Já existe uma pessoa cadastrada com o CPF '{personDTO.CPF}'.");
            }

            // Adicionar validações adicionais conforme necessário para outras propriedades

            // Mapear PersonDTO para a entidade Person
            var newPerson = new Person
            {
                Id = personDTO.Id,
                Name = personDTO.Name,
                Email = personDTO.Email,
                CPF = personDTO.CPF,
                BirthDate = personDTO.BirthDate,
                FkAddressId = personDTO.FkAddressId
            };

            // Adicionar a nova pessoa ao contexto
            _context.People.Add(newPerson);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Person name '{newPerson.Name}' recorded successfully.");

            // Retornar a nova pessoa criada
            return Ok(newPerson);
        }




        // READ - Método GET (Todos) para Person
        [HttpGet]
        public IActionResult GetAllPersons()
        {
            // Obter todas as pessoas
            var persons = _context.People.ToList();

            // Verificar se há pessoas no banco de dados
            if (persons == null || persons.Count == 0)
            {
                throw new NotFoundException("Nenhuma pessoa encontrada.");
            }

            // Mapear Person para PersonDTO
            var personDtoList = persons.Select(p => new PersonDTO
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                BirthDate = p.BirthDate,
                CPF = p.CPF,
                FkAddressId = p.FkAddressId,

            }).ToList();

            return Ok(personDtoList);
        }


        // READ - Método GET por Id para Person
        [HttpGet("{id}")]
        public IActionResult GetPersonById(int id)
        {
            // Obter a pessoa com o Id fornecido
            var person = _context.People.FirstOrDefault(p => p.Id == id);

            // Verificar se a pessoa foi encontrada
            if (person == null)
            {
                throw new NotFoundException("Pessoa não encontrada.");
            }

            // Mapear Person para PersonDTO
            var personDto = new PersonDTO
            {
                Id = person.Id,
                Name = person.Name,
                Email = person.Email,
                CPF = person.CPF,
                BirthDate = person.BirthDate,
                FkAddressId = person.FkAddressId,
            };

            return Ok(personDto);
        }


        // UPDATE - Método PUT para Person
        [HttpPut("{id}")]
        public IActionResult UpdatePerson(int id, [FromBody] PersonDTO personDTO)
        {

            // Obter a pessoa com o Id fornecido
            var person = _context.People.FirstOrDefault(p => p.Id == id);

            // Verificar se a pessoa foi encontrada
            if (person == null)
            {
                throw new NotFoundException("Pessoa não encontrada.");
            }

            // Validar se o CPF é único
            if (_context.People.Any(p => p.CPF == personDTO.CPF))
            {
                throw new InvalidDataException($"Já existe uma pessoa cadastrada com o CPF '{personDTO.CPF}'.");
            }

            // Atualizar propriedades da pessoa
            person.Name = personDTO.Name;
            person.Email = personDTO.Email;
            person.CPF = personDTO.CPF;
            person.BirthDate = personDTO.BirthDate;
            person.FkAddressId = personDTO.FkAddressId;


            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Person id {id} updated successfully.");

            return Ok(person);

        }


        // DELETE - Método DELETE para Person
        [HttpDelete("{id}")]
        public IActionResult DeletePerson(int id)
        {
            // Obter a pessoa com o Id fornecido
            var person = _context.People.FirstOrDefault(p => p.Id == id);

            // Verificar se a pessoa foi encontrada
            if (person == null)
            {
                throw new NotFoundException("Pessoa não encontrada.");
            }

            // Remover a pessoa do contexto
            _context.People.Remove(person);

            // Remover o endereço associado se existir
            if (person.FkAddress != null)
            {
                _context.Addresses.Remove(person.FkAddress);
            }

            // Salvar as alterações no banco de dados
            _context.SaveChanges();
            _logger.WriteLogData($"Person id {id} deleted successfully.");

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

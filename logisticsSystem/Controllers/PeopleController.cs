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


        // GET geral para Person
        [HttpGet]
        public IActionResult GetAllPersons()
        {
            var persons = _context.People.ToList();

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


        // GET para Person por id
        [HttpGet("{id}")]
        public IActionResult GetPersonById(int id)
        {
            var person = _context.People.FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                throw new NotFoundException("Pessoa não encontrada.");
            }

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


        [HttpPost]
        public IActionResult CreatePerson([FromBody] PersonDTO personDTO)
        {
            if (personDTO == null)
            {
                throw new NullRequestException("Solicitação inválida para criação de pessoa.");
            }

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

            var newPerson = new Person
            {
                Id = personDTO.Id,
                Name = personDTO.Name,
                Email = personDTO.Email,
                CPF = personDTO.CPF,
                BirthDate = personDTO.BirthDate,
                FkAddressId = personDTO.FkAddressId
            };

            _context.People.Add(newPerson);

            _context.SaveChanges();
            _logger.WriteLogData($"Person de nome:  '{newPerson.Name}' registrada com sucesso.");

            return Ok(newPerson);
        }


        [HttpPut("{id}")]
        public IActionResult UpdatePerson(int id, [FromBody] PersonDTO personDTO)
        {
            var person = _context.People.FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                throw new NotFoundException($"Person de Id: {id} não encontrado.");
            }

            // Validar se o CPF é único
            if (_context.People.Any(p => p.CPF == personDTO.CPF))
            {
                throw new InvalidDataException($"Já existe uma pessoa cadastrada com o CPF '{personDTO.CPF}'.");
            }

            person.Name = personDTO.Name;
            person.Email = personDTO.Email;
            person.CPF = personDTO.CPF;
            person.BirthDate = personDTO.BirthDate;
            person.FkAddressId = personDTO.FkAddressId;

            _context.SaveChanges();
            _logger.WriteLogData($"Person id {id} updated successfully.");

            return Ok(person);
        }


        [HttpDelete("{id}")]
        public IActionResult DeletePerson(int id)
        {
            var person = _context.People.FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                throw new NotFoundException("Pessoa não encontrada.");
            }

            _context.People.Remove(person);

            if (person.FkAddress != null)
            {
                _context.Addresses.Remove(person.FkAddress);
            }

            _context.SaveChanges();
            _logger.WriteLogData($"Person de id: {id} deletado com sucesso.");

            return NoContent(); 
        }
    }
}

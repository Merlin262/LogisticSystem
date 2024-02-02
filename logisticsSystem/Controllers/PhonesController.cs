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

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhonesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public PhonesController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // CREATE - Método POST para Phone
        [HttpPost]
        public IActionResult CreatePhone([FromBody] PhoneDTO phoneDTO)
        {
            // Mapear PhoneDTO para a entidade Phone
            var newPhone = new Phone
            {
                Id = phoneDTO.Id,
                AreaCode = phoneDTO.AreaCode,
                Number = phoneDTO.Number,
                FkPersonId = phoneDTO.FkPersonId
            };

            // Adicionar o novo telefone ao contexto
            _context.Phones.Add(newPhone);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o novo telefone criado
            return Ok(newPhone);
        }

        // READ - Método GET (Todos) para Phone
        [HttpGet]
        public IActionResult GetAllPhones()
        {
            // Obter todos os telefones
            var phones = _context.Phones.ToList();

            // Mapear Phone para PhoneDTO
            var phoneDtoList = phones.Select(p => new PhoneDTO
            {
                Id = p.Id,
                AreaCode = p.AreaCode,
                Number = p.Number,
                FkPersonId = p.FkPersonId
            }).ToList();

            return Ok(phoneDtoList);
        }

        // READ - Método GET por Id para Phone
        [HttpGet("{id}")]
        public IActionResult GetPhoneById(int id)
        {
            // Obter o telefone com o Id fornecido
            var phone = _context.Phones.FirstOrDefault(p => p.Id == id);

            if (phone == null)
            {
                return NotFound(); // Retorna 404 Not Found se o telefone não for encontrado
            }

            // Mapear Phone para PhoneDTO
            var phoneDto = new PhoneDTO
            {
                Id = phone.Id,
                AreaCode = phone.AreaCode,
                Number = phone.Number,
                FkPersonId = phone.FkPersonId
            };

            return Ok(phoneDto);
        }

        // UPDATE - Método PUT para Phone
        [HttpPut("{id}")]
        public IActionResult UpdatePhone(int id, [FromBody] PhoneDTO phoneDTO)
        {
            // Obter o telefone com o Id fornecido
            var phone = _context.Phones.FirstOrDefault(p => p.Id == id);

            if (phone == null)
            {
                return NotFound(); // Retorna 404 Not Found se o telefone não for encontrado
            }

            // Atualizar propriedades do telefone
            phone.AreaCode = phoneDTO.AreaCode;
            phone.Number = phoneDTO.Number;
            phone.FkPersonId = phoneDTO.FkPersonId;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(phone);
        }

        // DELETE - Método DELETE para Phone
        [HttpDelete("{id}")]
        public IActionResult DeletePhone(int id)
        {
            // Obter o telefone com o Id fornecido
            var phone = _context.Phones.FirstOrDefault(p => p.Id == id);

            if (phone == null)
            {
                return NotFound(); // Retorna 404 Not Found se o telefone não for encontrado
            }

            // Remover o telefone do contexto
            _context.Phones.Remove(phone);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

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

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingPaymentsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public ShippingPaymentsController(LogisticsSystemContext context)
        {
            _context = context;
        }

        // GET: api/shipping-payments
        [HttpGet]
        public IActionResult GetShippingPayments()
        {
            var shippingPayments = _context.ShippingPayments.ToList();

            // Verificar se há pagamentos de envio no banco de dados
            if (shippingPayments == null || shippingPayments.Count == 0)
            {
                throw new NotFoundException("Nenhum pagamento de envio encontrado.");
            }

            var shippingPaymentDTOs = shippingPayments.Select(sp => new ShippingPaymentDTO
            {
                Id = sp.Id,
                PaymentDate = sp.PaymentDate,
                FkShippingId = sp.FkShippingId
            });

            return Ok(shippingPaymentDTOs);

        }


        // GET: api/shipping-payments/{id}
        [HttpGet("{id}")]
        public IActionResult GetShippingPaymentById(int id)
        {

            var shippingPayment = _context.ShippingPayments.FirstOrDefault(sp => sp.Id == id);

            if (shippingPayment == null)
            {
                throw new NotFoundException("Pagamento de envio não encontrado.");
            }

            var shippingPaymentDTO = new ShippingPaymentDTO
            {
                Id = shippingPayment.Id,
                PaymentDate = shippingPayment.PaymentDate,
                FkShippingId = shippingPayment.FkShippingId
            };

            return Ok(shippingPaymentDTO);

        }


        // POST: api/shipping-payments
        [HttpPost]
        public IActionResult CreateShippingPayment([FromBody] ShippingPaymentDTO shippingPaymentDTO)
        {

            if (shippingPaymentDTO == null)
            {
                throw new NullRequestException("Confira se não deixou nenhum campo em branco");
            }

            // Verificar se a data de pagamento é uma data válida
            if (shippingPaymentDTO.PaymentDate == default(DateOnly))
            {
                throw new InvalidDataException("A data de pagamento é inválida.");
            }

            // Verificar se o FkShippingId é um número positivo
            if (shippingPaymentDTO.FkShippingId <= 0)
            {
                throw new InvalidDataException("FkShippingId deve ser um número positivo.");
            }

            var shippingPayment = new ShippingPayment
            {
                PaymentDate = shippingPaymentDTO.PaymentDate,
                FkShippingId = shippingPaymentDTO.FkShippingId
            };

            _context.ShippingPayments.Add(shippingPayment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetShippingPaymentById), new { id = shippingPayment.Id }, shippingPaymentDTO);
        }






        // PUT: api/shipping-payments/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateShippingPayment(int id, [FromBody] ShippingPaymentDTO updatedShippingPaymentDTO)
        {

            var existingShippingPayment = _context.ShippingPayments.FirstOrDefault(sp => sp.Id == id);

            if (existingShippingPayment == null)
            {
                throw new NotFoundException("Pagamento de envio não encontrado.");
            }

            // Verificar se a data de pagamento é uma data válida
            if (updatedShippingPaymentDTO.PaymentDate == default(DateOnly))
            {
                throw new InvalidDataException("A data de pagamento é inválida.");
            }

            // Verificar se o FkShippingId é um número positivo
            if (updatedShippingPaymentDTO.FkShippingId <= 0)
            {
                throw new InvalidDataException("FkShippingId deve ser um número positivo.");
            }

            existingShippingPayment.PaymentDate = updatedShippingPaymentDTO.PaymentDate;
            existingShippingPayment.FkShippingId = updatedShippingPaymentDTO.FkShippingId;

            _context.SaveChanges();

            var updatedShippingPaymentResponse = new ShippingPaymentDTO
            {
                Id = existingShippingPayment.Id,
                PaymentDate = existingShippingPayment.PaymentDate,
                FkShippingId = existingShippingPayment.FkShippingId
            };

            return Ok(updatedShippingPaymentResponse);

        }


        // DELETE: api/shipping-payments/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteShippingPayment(int id)
        {
            var shippingPayment = _context.ShippingPayments.FirstOrDefault(sp => sp.Id == id);

            if (shippingPayment == null)
            {
                throw new NotFoundException("Pagamento de envio não encontrado.");
            }

            _context.ShippingPayments.Remove(shippingPayment);
            _context.SaveChanges();

            return NoContent();
        }

    }

}

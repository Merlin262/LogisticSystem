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
                return NotFound("Pagamento de envio não encontrado.");
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
                return NotFound("Pagamento de envio não encontrado.");
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
                return NotFound("Pagamento de envio não encontrado.");
            }

            _context.ShippingPayments.Remove(shippingPayment);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

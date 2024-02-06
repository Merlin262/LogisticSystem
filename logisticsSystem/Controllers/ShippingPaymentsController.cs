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
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingPaymentsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        private readonly LoggerService _logger;
        private readonly ReceiptService _receipt;

        public ShippingPaymentsController(LogisticsSystemContext context, LoggerService logger, ReceiptService receipt)
        {
            _context = context;
            _logger = logger;
            _receipt = receipt;
        }

        // GET geral para ShippingPayment
        [HttpGet]
        public IActionResult GetShippingPayments()
        {
            var shippingPayments = _context.ShippingPayments.ToList();

            if (shippingPayments == null || shippingPayments.Count == 0)
            {
                throw new NotFoundException($"Nenhum ShippingPayment encontrado no banco de dados.");
            }

            var shippingPaymentDTOs = shippingPayments.Select(sp => new ShippingPaymentDTO
            {
                Id = sp.Id,
                PaymentDate = sp.PaymentDate,
                FkShippingId = sp.FkShippingId
            });

            return Ok(shippingPaymentDTOs);

        }


        // GET para ShippingPayment por id
        [HttpGet("{id}")]
        public IActionResult GetShippingPaymentById(int id)
        {

            var shippingPayment = _context.ShippingPayments.FirstOrDefault(sp => sp.Id == id);

            if (shippingPayment == null)
            {
                throw new NotFoundException($"ShippingPayment de Id: {id} encontrado no banco de dados.");
            }

            var shippingPaymentDTO = new ShippingPaymentDTO
            {
                Id = shippingPayment.Id,
                PaymentDate = shippingPayment.PaymentDate,
                FkShippingId = shippingPayment.FkShippingId
            };

            return Ok(shippingPaymentDTO);

        }


        [HttpPost]
        public IActionResult CreateShippingPayment([FromBody] ShippingPaymentDTO shippingPaymentDTO)
        {

            if (shippingPaymentDTO == null)
            {
                throw new NullRequestException("Confira se não deixou nenhum campo em branco");
            }

            if (shippingPaymentDTO.PaymentDate == default(DateOnly))
            {
                throw new InvalidDataException("A data de pagamento é inválida.");
            }

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
            _logger.WriteLogData($"ShippingPayment de Id {shippingPayment.FkShippingId} registrado com sucesso.");
            _receipt.GenerateClientReceipt(shippingPayment.Id);

            return CreatedAtAction(nameof(GetShippingPaymentById), new { id = shippingPayment.Id }, shippingPaymentDTO);
        }






        [HttpPut("{id}")]
        public IActionResult UpdateShippingPayment(int id, [FromBody] ShippingPaymentDTO updatedShippingPaymentDTO)
        {

            var existingShippingPayment = _context.ShippingPayments.FirstOrDefault(sp => sp.Id == id);

            if (existingShippingPayment == null)
            {
                throw new NotFoundException($"ShippingPayment de Id: {id} encontrado no banco de dados.");
            }

            // Verificar se a data de pagamento é uma data válida
            if (updatedShippingPaymentDTO.PaymentDate == default(DateOnly))
            {
                throw new InvalidDataException("A data de pagamento é inválida.");
            }

            if (updatedShippingPaymentDTO.FkShippingId <= 0)
            {
                throw new InvalidDataException("FkShippingId deve ser um número positivo.");
            }

            existingShippingPayment.PaymentDate = updatedShippingPaymentDTO.PaymentDate;
            existingShippingPayment.FkShippingId = updatedShippingPaymentDTO.FkShippingId;

            _context.SaveChanges();
            _logger.WriteLogData($"ShippingPayment de Id: {existingShippingPayment.FkShippingId} atualizado com sucesso.");

            var updatedShippingPaymentResponse = new ShippingPaymentDTO
            {
                Id = existingShippingPayment.Id,
                PaymentDate = existingShippingPayment.PaymentDate,
                FkShippingId = existingShippingPayment.FkShippingId
            };

            return Ok(updatedShippingPaymentResponse);

        }


        [HttpDelete("{id}")]
        public IActionResult DeleteShippingPayment(int id)
        {
            var shippingPayment = _context.ShippingPayments.FirstOrDefault(sp => sp.Id == id);

            if (shippingPayment == null)
            {
                throw new NotFoundException($"ShippingPayment de Id: {id} encontrado no banco de dados.");
            }

            _context.ShippingPayments.Remove(shippingPayment);
            _context.SaveChanges();
            _logger.WriteLogData($"ShippingPayment de id: {id} deletado com sucesso.");

            return NoContent();
        }

    }

}

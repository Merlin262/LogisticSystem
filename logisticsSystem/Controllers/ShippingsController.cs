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
using logisticsSystem.Services;

namespace logisticsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingsController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;
        //Injeção de dependência do serviço TruckService
        private readonly TruckService _truckService;

        public ShippingsController(LogisticsSystemContext context, TruckService truckService)
        {
            _context = context;
            _truckService = truckService;
        }

        [HttpPost("create-shipping")]
        public IActionResult CreateShipping([FromBody] ShippingDTO shippingDTO)
        {
            // Mapear ShippingDTO para a entidade Shipping
            var newShipping = new Shipping
            {
                SendDate = shippingDTO.SendDate,
                EstimatedDate = shippingDTO.EstimatedDate,
                DeliveryDate = shippingDTO.DeliveryDate,
                TotalWeight = shippingDTO.TotalWeight,
                DistanceKm = shippingDTO.DistanceKm,
                RegistrationDate = shippingDTO.RegistrationDate,
                ShippingPrice = shippingDTO.ShippingPrice,
                FkClientId = shippingDTO.FkClientId,
                FkEmployeeId = shippingDTO.FkEmployeeId,
                FkAddressId = shippingDTO.FkAddressId,
                FkTruckId = shippingDTO.FkTruckId // Adicione esta propriedade se não existir
            };

            // Verificar se o caminhão pode lidar com o peso do pedido
            if (!_truckService.CanTruckHandleShipping(shippingDTO))
            {
                return BadRequest("O caminhão não pode lidar com o peso do pedido.");
            }

            // Adicionar o novo envio ao contexto
            _context.Shippings.Add(newShipping);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o novo envio criado
            return Ok(newShipping);
        }
    

    // READ - Método GET (Todos) para Shipping
    [HttpGet]
        public IActionResult GetAllShippings()
        {
            // Obter todos os envios
            var shippings = _context.Shippings.ToList();

            // Mapear Shipping para ShippingDTO
            var shippingDtoList = shippings.Select(s => new ShippingDTO
            {
                Id = s.Id,
                SendDate = s.SendDate,
                EstimatedDate = s.EstimatedDate,
                DeliveryDate = s.DeliveryDate,
                TotalWeight = s.TotalWeight,
                DistanceKm = s.DistanceKm,
                RegistrationDate = s.RegistrationDate,
                ShippingPrice = s.ShippingPrice,
                FkClientId = s.FkClientId,
                FkEmployeeId = s.FkEmployeeId,
                FkAddressId = s.FkAddressId
            }).ToList();

            return Ok(shippingDtoList);
        }

        // READ - Método GET por Id para Shipping
        [HttpGet("{id}")]
        public IActionResult GetShippingById(int id)
        {
            // Obter o envio com o Id fornecido
            var shipping = _context.Shippings.FirstOrDefault(s => s.Id == id);

            if (shipping == null)
            {
                return NotFound(); // Retorna 404 Not Found se o envio não for encontrado
            }

            // Mapear Shipping para ShippingDTO
            var shippingDto = new ShippingDTO
            {
                Id = shipping.Id,
                SendDate = shipping.SendDate,
                EstimatedDate = shipping.EstimatedDate,
                DeliveryDate = shipping.DeliveryDate,
                TotalWeight = shipping.TotalWeight,
                DistanceKm = shipping.DistanceKm,
                RegistrationDate = shipping.RegistrationDate,
                ShippingPrice = shipping.ShippingPrice,
                FkClientId = shipping.FkClientId,
                FkEmployeeId = shipping.FkEmployeeId,
                FkAddressId = shipping.FkAddressId
            };

            return Ok(shippingDto);
        }

        // UPDATE - Método PUT para Shipping
        [HttpPut("{id}")]
        public IActionResult UpdateShipping(int id, [FromBody] ShippingDTO shippingDTO)
        {
            // Obter o envio com o Id fornecido
            var shipping = _context.Shippings.FirstOrDefault(s => s.Id == id);

            if (shipping == null)
            {
                return NotFound(); // Retorna 404 Not Found se o envio não for encontrado
            }

            // Atualizar propriedades do envio
            shipping.SendDate = shippingDTO.SendDate;
            shipping.EstimatedDate = shippingDTO.EstimatedDate;
            shipping.DeliveryDate = shippingDTO.DeliveryDate;
            shipping.TotalWeight = shippingDTO.TotalWeight;
            shipping.DistanceKm = shippingDTO.DistanceKm;
            shipping.RegistrationDate = shippingDTO.RegistrationDate;
            shipping.ShippingPrice = shippingDTO.ShippingPrice;
            shipping.FkClientId = shippingDTO.FkClientId;
            shipping.FkEmployeeId = shippingDTO.FkEmployeeId;
            shipping.FkAddressId = shippingDTO.FkAddressId;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(shipping);
        }

        // DELETE - Método DELETE para Shipping
        [HttpDelete("{id}")]
        public IActionResult DeleteShipping(int id)
        {
            // Obter o envio com o Id fornecido
            var shipping = _context.Shippings.FirstOrDefault(s => s.Id == id);

            if (shipping == null)
            {
                return NotFound(); // Retorna 404 Not Found se o envio não for encontrado
            }

            // Remover o envio do contexto
            _context.Shippings.Remove(shipping);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão
        }
    }
}

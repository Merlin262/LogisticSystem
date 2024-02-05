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
        private readonly ItensShippedService _itensShippedService;
        private readonly EmployeeWageService _employeeWageService;

        public ShippingsController(LogisticsSystemContext context, TruckService truckService, ItensShippedService itensShippedService, EmployeeWageService employeeWageService)
        {
            _context = context;
            _truckService = truckService;
            _itensShippedService = itensShippedService;
            _employeeWageService = employeeWageService;
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
                FkTruckId = shippingDTO.FkTruckId// Adicione esta propriedade se não existir
            };

            // Adicionar o novo envio ao contexto
            _context.Shippings.Add(newShipping);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            decimal employeeComission = _employeeWageService.GetEmployeeComission(newShipping.Id);

            var employeeToUpdate = _context.EmployeeWages.FirstOrDefault(e => e.FkEmployeeId == shippingDTO.FkEmployeeId);
            if (employeeToUpdate == null)
            {
                return NotFound("Employee not found");
            }

            employeeToUpdate.Commission = employeeToUpdate.Commission + employeeComission;

            _context.SaveChanges();

            // Retornar o novo envio criado, evitando referências circulares
            return Ok("Pedido criado");
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
                FkAddressId = s.FkAddressId,
                FkTruckId = s.FkTruckId
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


        [HttpPut("update-shipping/{id}")]
        public IActionResult UpdateShipping(int id, [FromBody] ShippingDTO updatedShippingDTO)
        {
            // Verificar se o envio com o id fornecido existe
            var existingShipping = _context.Shippings.Find(id);

            if (existingShipping == null)
            {
                return NotFound();
            }

            // Atualizar as propriedades do envio existente com base no DTO fornecido
            existingShipping.SendDate = updatedShippingDTO.SendDate;
            existingShipping.EstimatedDate = updatedShippingDTO.EstimatedDate;
            existingShipping.DeliveryDate = updatedShippingDTO.DeliveryDate;
            existingShipping.TotalWeight = updatedShippingDTO.TotalWeight;
            existingShipping.DistanceKm = updatedShippingDTO.DistanceKm;
            existingShipping.RegistrationDate = updatedShippingDTO.RegistrationDate;
            existingShipping.ShippingPrice = updatedShippingDTO.ShippingPrice;
            existingShipping.FkClientId = updatedShippingDTO.FkClientId;
            existingShipping.FkEmployeeId = updatedShippingDTO.FkEmployeeId;
            existingShipping.FkAddressId = updatedShippingDTO.FkAddressId;
            existingShipping.FkTruckId = updatedShippingDTO.FkTruckId; // Adicione esta propriedade se não existir

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar o envio atualizado, evitando referências circulares
            return Ok(new
            {
                existingShipping.Id,
                existingShipping.SendDate,
                // Adicione outras propriedades relevantes de Shipping
                // ...

                FkTruck = new
                {
                    existingShipping.FkTruck.Chassis,
                    existingShipping.FkTruck.TruckAxles,
                    // Adicione outras propriedades relevantes de Truck
                    // ...
                }
            });
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

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
    public class MaintenancesController : ControllerBase
    {
        private readonly LogisticsSystemContext _context;

        public MaintenancesController(LogisticsSystemContext context)
        {
            _context = context;
        }

        



        // READ - Método GET (Todos) para Maintenance
        [HttpGet]
        public IActionResult GetAllMaintenance()
        {

            // Obter todas as manutenções
            var maintenances = _context.Maintenances.ToList();

            // Verificar se há manutenções no banco de dados
            if (maintenances == null || maintenances.Count == 0)
            {
                throw new NotFoundException("Nenhuma manutenção encontrada.");
            }

            // Mapear Maintenance para MaintenanceDTO
            var maintenanceDtoList = maintenances.Select(m => new MaintenanceDTO
            {
                Id = m.Id,
                MaintenanceDate = m.MaintenanceDate,
                FkEmployee = m.FkEmployee,
                FkTruckChassis = m.FkTruckChassis
            }).ToList();

            return Ok(maintenanceDtoList);

        }


        [HttpGet("{id}")]
        public IActionResult GetMaintenanceById(int id)
        {
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            if (maintenance == null)
            {
                throw new NotFoundException("Manutenção não encontrada.");
            }

            var maintenanceDto = new MaintenanceDTO
            {
                Id = maintenance.Id,
                MaintenanceDate = maintenance.MaintenanceDate,
                FkEmployee = maintenance.FkEmployee,
                FkTruckChassis = maintenance.FkTruckChassis
            };

            return Ok(maintenanceDto);
        }




        [HttpPost]
        public IActionResult CreateMaintenance([FromBody] MaintenanceDTO maintenanceDTO)
        {
            
            // Verificar se a solicitação é nula
            if (maintenanceDTO == null)
            {
                throw new NullRequestException("Solicitação inválida para criação de manutenção.");
            }

            // Verificar se a data de manutenção é uma data válida
            if (maintenanceDTO.MaintenanceDate == default)
            {
                throw new InvalidDataTypeException("Data de manutenção inválida.");
            }

            // Verificar se o funcionário associado à manutenção existe
            var employeeExists = _context.Employees.Any(e => e.FkPersonId == maintenanceDTO.FkEmployee);
            if (!employeeExists)
            {
                throw new NotFoundException("Funcionário associado à manutenção não encontrado.");
            }

            // Verificar se o chassi do caminhão associado à manutenção existe
            var truckChassisExists = _context.Trucks.Any(tc => tc.Chassis == maintenanceDTO.FkTruckChassis);

            if (!truckChassisExists)
            {
                throw new NotFoundException("Chassi do caminhão associado à manutenção não encontrado.");
            }

            // Mapear MaintenanceDTO para a entidade Maintenance
            var newMaintenance = new Maintenance
            {
                MaintenanceDate = maintenanceDTO.MaintenanceDate,
                FkEmployee = maintenanceDTO.FkEmployee,
                FkTruckChassis = maintenanceDTO.FkTruckChassis
            };

            // Adicionar a nova manutenção ao contexto
            _context.Maintenances.Add(newMaintenance);

            // Atualizar o caminhão para indicar que está em manutenção
            var truckToUpdate = _context.Trucks.FirstOrDefault(tc => tc.Chassis == maintenanceDTO.FkTruckChassis);

            if (truckToUpdate != null)
            {
                truckToUpdate.LastMaintenanceKilometers = 0; // Definir LastMaintenance como 0
                truckToUpdate.InMaintenance = true;
            }

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            // Retornar a nova manutenção criada
            return Ok(newMaintenance);
            
        }



        // UPDATE - Método PUT para Maintenance
        [HttpPut("{id}")]
        public IActionResult UpdateMaintenance(int id, [FromBody] MaintenanceDTO maintenanceDTO)
        {

            // Obter a manutenção com o Id fornecido
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            // Verificar se a manutenção foi encontrada
            if (maintenance == null)
            {
                throw new NotFoundException("Manutenção não encontrada.");
            }

            // Validar propriedades da manutenção antes da atualização
            if (maintenanceDTO.MaintenanceDate == default(DateOnly))
            {
                throw new InvalidDataException("A data da manutenção não pode ser nula ou padrão.");
            }

            // Adicione mais verificações conforme necessário para outras propriedades

            // Verificar se o caminhão existe pelo chassi inserido
            var truck = _context.Trucks.FirstOrDefault(t => t.Chassis == maintenanceDTO.FkTruckChassis);

            if (truck == null)
            {
                throw new NotFoundException("Caminhão não encontrado pelo chassi fornecido.");
            }

            // Atualizar propriedades da manutenção
            maintenance.MaintenanceDate = maintenanceDTO.MaintenanceDate;
            maintenance.FkEmployee = maintenanceDTO.FkEmployee;
            maintenance.FkTruckChassis = maintenanceDTO.FkTruckChassis;

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return Ok(maintenance);

        }


        // DELETE - Método DELETE para Maintenance
        [HttpDelete("{id}")]
        public IActionResult DeleteMaintenance(int id)
        {
            // Obter a manutenção com o Id fornecido
            var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);

            // Verificar se a manutenção foi encontrada
            if (maintenance == null)
            {
                throw new NotFoundException("Manutenção não encontrada.");
            }

            // Remover a manutenção do contexto
            _context.Maintenances.Remove(maintenance);

            // Salvar as alterações no banco de dados
            _context.SaveChanges();

            return NoContent(); // Retorna 204 No Content para indicar sucesso na exclusão

        }
    }
}

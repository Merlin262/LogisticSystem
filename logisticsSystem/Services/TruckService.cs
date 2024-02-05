using logisticsSystem.Controllers.logisticsSystem.Controllers;
using logisticsSystem.Data;
using logisticsSystem.DTOs;
using logisticsSystem.Controllers;
using logisticsSystem.Exceptions;
using logisticsSystem.Services;

namespace logisticsSystem.Services
{
    public class TruckService
    {
        private readonly LogisticsSystemContext _context;

        public TruckService(LogisticsSystemContext context, ItensShippedService itensShippedService)
        {
            _context = context;
        }

        public int GetTruckAxlesWeight(int fkShippingId)
        {
            var truckAxles = _context.Shippings
                .Where(s => s.Id == fkShippingId)
                .Select(s => s.FkTruck.TruckAxles)
                .FirstOrDefault();

            return truckAxles * 2000;
        }

        public bool UpdateTruckMaintenanceStatus(int truckId, decimal distance)
        {
            // 50000 km é o limite para manutenção
            const int maintenanceThreshold = 50000;

            // Obter o caminhão com o truckId fornecido do contexto
            var truck = _context.Trucks.FirstOrDefault(t => t.Chassis == truckId);

            if (truck != null)
            {

                if (truck.InMaintenance)
                {
                    // Lançar exceção se o caminhão já estiver em manutenção
                    throw new InvalidTruckException("O caminhão já está em manutenção.");
                }

                // Verificar se LastMaintenanceKilometers é superior ao limite
                if (truck.LastMaintenanceKilometers + distance  >= maintenanceThreshold)
                {
                    // Atualizar o booleano InMaintenance para true
                    truck.InMaintenance = true;
                }
                else
                {
                    // Caso contrário, definir InMaintenance como false
                    truck.InMaintenance = false;
                }

                // Salvar as alterações no banco de dados
                _context.SaveChanges();
            }
            // Você pode fazer outras verificações ou lógica específica para o serviço aqui
            return truck.InMaintenance;
        }




    }
}

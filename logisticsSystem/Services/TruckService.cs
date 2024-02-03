using logisticsSystem.Data;
using logisticsSystem.DTOs;

namespace logisticsSystem.Services
{
    public class TruckService
    {
        private readonly LogisticsSystemContext _context; // Substitua por seu DbContext real

        public TruckService(LogisticsSystemContext context)
        {
            _context = context;
        }

        public bool CanTruckHandleShipping(ShippingDTO shippingDTO)
        {
            var truck = _context.Trucks.FirstOrDefault(t => t.Chassis == shippingDTO.FkTruckId);

            if (truck == null)
            {
                // Caminhão não encontrado
                return false;
            }

            // Verificar se o número de eixos do caminhão é suficiente para o peso do pedido
            return truck.TruckAxles >= CalculateRequiredAxles(shippingDTO.TotalWeight);
        }

        private int CalculateRequiredAxles(decimal totalWeight)
        {
            // Lógica para calcular o número de eixos necessário com base no peso total
            // Pode ser uma lógica simples ou mais complexa dependendo dos requisitos específicos do seu sistema
            // Neste exemplo, assumimos que cada eixo pode suportar até 5 toneladas
            const decimal maxWeightPerAxle = 5.0m;
            return (int)Math.Ceiling(totalWeight / maxWeightPerAxle);
        }
    }
}

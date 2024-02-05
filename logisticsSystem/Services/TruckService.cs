using logisticsSystem.Controllers.logisticsSystem.Controllers;
using logisticsSystem.Data;
using logisticsSystem.DTOs;
using logisticsSystem.Controllers;
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

    }
}

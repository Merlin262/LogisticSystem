using logisticsSystem.Data;
using logisticsSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace logisticsSystem.Services
{
    public class ItensShippedService
    {
        private readonly LogisticsSystemContext _context;

        public ItensShippedService(LogisticsSystemContext context)
        {
            _context = context;
        }

        // Recebe o Peso Total dos Itens de um Pedido
        public decimal GetTotalItemWeight(int fkItensShippedId)
        {
            var totalWeight = _context.ItensShippeds
                .Where(i => i.Id == fkItensShippedId)
                .Join(
                    _context.ItensStocks,
                    i => i.FkItensStockId,
                    s => s.Id,
                    (i, s) => i.QuantityItens * s.Weight)
                .FirstOrDefault();

            return totalWeight;
        }



    }
}

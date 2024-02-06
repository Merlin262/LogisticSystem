using logisticsSystem.Data;
using logisticsSystem.Models;
using Newtonsoft.Json.Linq;

namespace logisticsSystem.Services
{
    public class ReceiptService
    {
        private readonly LogisticsSystemContext _context;

        public ReceiptService(LogisticsSystemContext context)
        {
            _context = context;
        }

        public void GenerateClientReceipt(int shippingPaymentId)
        {
            string filePath = $"C:\\Users\\joaom\\OneDrive\\Documentos\\Volvo\\Clone5002\\Receipts\\ReciboCliente{shippingPaymentId}.txt";
            if (!File.Exists(filePath))
            {
                // Cria o arquivo e fecha imediatamente
                using (File.Create(filePath)) { }
            }

            var dataReceipt = _context.ShippingPayments
                .Where(sp => sp.Id == shippingPaymentId)
                .Join(
                    _context.Shippings,
                    sp => sp.FkShippingId,
                    s => s.Id,
                    (sp, s) => new { ShippingPayment = sp, Shipping = s }
                ).Join(
                    _context.Clients,
                    s => s.Shipping.FkClientId,
                    c => c.FkPersonId,
                    (s, c) => new { Client = c, value = s.Shipping.ShippingPrice }
                ).Join(
                    _context.People,
                    c => c.Client.FkPersonId,
                    p => p.Id,
                    (c, p) => new { name = p.Name, c.value }
                ).FirstOrDefault();

            string name = dataReceipt?.name ?? ""; // Use um operador de coalescência nula para garantir que não seja nulo
            decimal value = dataReceipt?.value ?? 0;

            using (var file = File.AppendText(filePath))
            {
                file.WriteLine($"|-------------------RECIBO N° {shippingPaymentId}-------------------|");
                file.WriteLine("         VALOR         CLIENTE         DATA ");
                file.WriteLine($"        R${value}       {name}        {DateTime.Now.ToString("dd/MM/yyyy")}");
                file.WriteLine("|--------------------------------------------------|");
            }

        }
    }
}

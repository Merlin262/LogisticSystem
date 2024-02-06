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
            string filePath = $"E:\\codes\\receiptsClients\\{shippingPaymentId}.txt";
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
            }

            /*
             * Faz intersecções entre as tabelas de ShippingPayments, Shippings, Clients e Person para resgatar os dados 
             * necessários para geração do recibo: valor (Shippings) e nome do cliente (Person)
             */
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

            string name = dataReceipt.name;
            decimal value = dataReceipt.value;

            using (var file = File.AppendText(filePath))
            {
                file.WriteLine($"|-------------------RECIBO N° {shippingPaymentId}-------------------|");
                file.WriteLine("         VALOR         CLIENTE         DATA ");
                file.WriteLine($"        R${value}       {name}        {DateTime.Now.ToString("dd/MM/yyyy")}");
                file.WriteLine("|--------------------------------------------------|");
            }

        }

        public void GenerateEmployeeReceipt(int employeeId, decimal netSalary)
        {
            //Obtém nome e cpf do employee fazendo o join com person
            var employee = _context.Employees
                .Where(e => e.FkPersonId == employeeId)
                .Join(
                    _context.People,
                    e => e.FkPersonId, 
                    p => p.Id,
                    (e, p) => new { Person = p }
                )
                .Select(x => new {
                    Name = x.Person.Name,
                    CPF = x.Person.CPF,
                    }
                )
                .FirstOrDefault();

            //Obtém os dados do pagamento fazendo a intesecção entre EmployeeWages, WageDeductions e gera uma lista de descontos
            //fazendo a intersecção com a tabela Deductions
            var employeeSalaryData = _context.EmployeeWages
                .Where(ew => ew.FkEmployeeId == employeeId)
                .Select(ew => new {
                    WageId = ew.Id,
                    GrossSalary = ew.Amount,
                    PayDay = ew.PayDay,
                    Commission = ew.Commission,
                    Deductions = _context.WageDeductions
                        .Where(wd => wd.FkWageId == ew.Id)
                        .Join(
                            _context.Deductions,
                            wd => wd.FkDeductionsId,
                            d => d.Id,
                            (wd, d) => new {
                                DeductionName = d.Name,
                                DeductionAmount = d.Amount
                            }
                        ).ToList()
                })
                .FirstOrDefault();

            //Atribui para variáveis todos os atributos resgatados na consulta  SQL
            string name = employee.Name;
            string cpf = employee.CPF;
            DateOnly payDay = employeeSalaryData.PayDay;
            int wageId = employeeSalaryData.WageId;
            decimal grossSalary = employeeSalaryData.GrossSalary;
            decimal commission = employeeSalaryData.Commission;

            //Cria o arquivo a partir do CPF + mês de pagamento, de forma que em todos os meses possa ser gerado um novo recibo 
            //irrepetível para cada funcionário
            string filePath = $"E:\\codes\\receiptsEmployees\\{cpf}_{payDay.Month}-{payDay.Year}.txt";
            using (File.Create(filePath)) { }

            using (var file = File.AppendText(filePath))
            {
                file.WriteLine($"*****************RECIBO DE PAGAMENTO DE SALÁRIO N° {wageId}*****************");
                file.WriteLine($"");
                file.WriteLine($"FUNCIONÁRIO: {name}");
                file.WriteLine($"CPF: {cpf}");
                file.WriteLine($"");
                file.WriteLine($"DETALHES DO PAGAMENTO:");
                file.WriteLine($"   SALÁRIO BRUTO: R$ {grossSalary}");
                file.WriteLine($"   COMISSÃO: R$ {commission}");
                file.WriteLine($"   DEDUÇÕES:");

                //Percorre todas a lista de descontos e registra cada um no recibo
                employeeSalaryData.Deductions.ForEach(d =>
                {
                    file.WriteLine($"       {d.DeductionName}: R$ {d.DeductionAmount}");
                });

                file.WriteLine($"   SALÁRIO LÍQUIDO: {netSalary}");
                file.WriteLine($"   DATA: {payDay}");
                file.WriteLine($"");
                file.WriteLine("**********************************************************************");
            }

        }
    }
}

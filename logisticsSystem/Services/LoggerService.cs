using logisticsSystem.Data;
using Microsoft.Extensions.Logging;

namespace logisticsSystem.Services
{
    public class LoggerService
    {
        private readonly LogisticsSystemContext _context;

        public LoggerService(LogisticsSystemContext context)
        {
            _context = context;
        }

        public void WriteLogError(string message)
        {
            string logDirectory = "C:\\Users\\joaom\\OneDrive\\Documentos\\Volvo\\Clone5002\\erros";
            string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_ErrorLog.txt";
            string logPath = Path.Combine(logDirectory, logFileName);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
                //writer.WriteLine($"Stack Trace: {exception.StackTrace}");
                writer.WriteLine(); // Adiciona uma linha em branco para separar os logs
            }
        }

        public void WriteLogData(string message)
        {
            string logDirectory = "C:\\Users\\joaom\\OneDrive\\Documentos\\Volvo\\Clone5002\\erros";
            string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_DataLog.txt";
            string logPath = Path.Combine(logDirectory, logFileName);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }

}

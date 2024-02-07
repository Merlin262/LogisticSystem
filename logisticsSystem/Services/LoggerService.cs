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

        // Escrever log de erro no diretório indicado (logDirectory)
        public void WriteLogError(string message)
        {
            try
            {
                string logDirectory = "E:\\codes\\logisticsSystem\\logisticsSystem\\Logs\\";
                string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_ErrorLog.txt"; //Cria um novo log para cada dia
                string logPath = Path.Combine(logDirectory, logFileName);

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                //Registra no arquivo o erro recebido, bem como a data e hora 
                using (StreamWriter writer = new StreamWriter(logPath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                    writer.WriteLine(); 
                }
            } catch(Exception ex)
            {
                throw new Exception("Houve um erro ao tentar manipular o arquivo de log: " + ex.ToString());
            }
        }

        // Escrever log de dados no diretório indicado (logDirectory)
        public void WriteLogData(string message)
        {
            try
            {
                string logDirectory = "E:\\codes\\logisticsSystem\\logisticsSystem\\Logs\\";
                string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_DataLog.txt";
                string logPath = Path.Combine(logDirectory, logFileName);

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                //Registra no arquivo o evento recebido, bem como a data e hora 
                using (StreamWriter writer = new StreamWriter(logPath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }catch(Exception ex)
            {
                throw new Exception("Houve um erro ao tentar manipular o arquivo de log: " + ex.ToString());
            }

        }
    }

}

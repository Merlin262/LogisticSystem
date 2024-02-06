﻿using logisticsSystem.Data;
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
            try
            {
                string logDirectory = "E:\\codes\\logisticsSystem\\logisticsSystem\\Logs\\";
                string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_ErrorLog.txt"; //Cria um novo log para cada dia
                string logPath = Path.Combine(logDirectory, logFileName);

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

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

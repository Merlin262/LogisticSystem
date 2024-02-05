﻿using logisticsSystem.Data;
using Microsoft.Extensions.Logging;

namespace logisticsSystem.Services
{
    public class ErrorLoggerService
    {
        private readonly LogisticsSystemContext _context;

        public ErrorLoggerService(LogisticsSystemContext context)
        {
            _context = context;
        }
        public void WriteLog(string message)
        {
            string logDirectory = "E:\\codes\\logisticsSystem\\logisticsSystem\\Logs";
            string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_ErrorLog.txt";
            string logPath = Path.Combine(logDirectory, logFileName);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message} \n ");
            }
        }

        public void WriteLogData(string message)
        {
            string logDirectory = "E:\\codes\\logisticsSystem\\logisticsSystem\\Logs";
            string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_DataLog.txt";
            string logPath = Path.Combine(logDirectory, logFileName);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            using (StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message} \n ");
            }
        }
    }
}

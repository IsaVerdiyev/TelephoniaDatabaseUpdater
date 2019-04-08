using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Threading;
using TelephoniaDatabaseUpdaterCore.Models;
using TelephoniaDatabaseUpdaterCore.Services;

namespace TelephoniaDatabaseUpdaterCore
{
    class Program
    {
        public static Timer timer;
        static void Main(string[] args)
        {
            //Console.WriteLine($"Program started {DateTime.Now}");
            timer = new Timer(ExecuteUpdateInTimer, null, Timeout.Infinite, Timeout.Infinite);
            timer.Change(0, Timeout.Infinite);
            while (true) { Thread.Sleep(1000); }


        }

        public static void ExecuteUpdateInTimer(object obj)
        {
            ILogger logger = new FileConsoleLogger();
            new DatabaseUpdater(logger , new CsvFileService(), new AsterixApiService(),  new SqlDataBaseService(), new CsvWorkdersService()).UpdateDatabase();
            if (!int.TryParse(ConfigurationManager.AppSettings["MinutesOfTimer"], out int minutes))
            {
                minutes = 5;
                
                logger.Log("Couldn't parse MinutesOfTimer value to int");
            }
            timer.Change(minutes * 60000, Timeout.Infinite);
        }
    }
}

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
            timer = new Timer(ExecuteUpdateInTimer, null, Timeout.Infinite, Timeout.Infinite);
            timer.Change(0, Timeout.Infinite);
            Console.ReadLine();
        }

        public static void ExecuteUpdateInTimer(object obj) {
            new DatabaseUpdater().UpdateDatabase();
            if(!int.TryParse(ConfigurationManager.AppSettings["MinutesOfTimer"], out int minutes))
            {
                minutes = 5;
                ILogger logger = new FileLogger();
                logger.Log("Couldn't parse MinutesOfTimer value to int");
            }
            timer.Change(minutes * 60000, Timeout.Infinite);
        }
    }
}

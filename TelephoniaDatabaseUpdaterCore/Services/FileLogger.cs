using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace TelephoniaDatabaseUpdaterCore.Services
{
    class FileLogger : ILogger
    {

        public void Log(string logContent)
        {
            using(StreamWriter stream = new StreamWriter(ConfigurationManager.AppSettings["LogFilePath"], true))
            {
                stream.WriteLine(logContent);
            }
        }

    }
}

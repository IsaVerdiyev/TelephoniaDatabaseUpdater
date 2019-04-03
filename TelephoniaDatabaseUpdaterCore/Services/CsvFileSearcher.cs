using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace TelephoniaDatabaseUpdaterCore.Services
{
    class CsvFileSearcher
    {

        string CsvFolderPath => ConfigurationManager.AppSettings["CsvFolderPath"];


        public string GetCsvFileFromFolderIfFoundOne()
        {

            List<string> files = Directory.GetFiles(CsvFolderPath, "*.*", SearchOption.TopDirectoryOnly).Where(n => n.Contains(".csv")).ToList();
           
            if(files.Count > 1)
            {
                throw new Exception($"Error: {DateTime.Now}: There are more csv files in folder {CsvFolderPath}. Should be 1");
            }
            else if(files.Count == 0)
            {
                throw new Exception($"Error: {DateTime.Now}: csv file was not found in folder {CsvFolderPath}");
            }

            return files.First();
        }
    }
}

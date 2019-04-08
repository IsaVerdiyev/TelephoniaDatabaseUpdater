using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace TelephoniaDatabaseUpdaterCore.Services
{
    class CsvFileService
    {
        public string GetCsvFileFromFolderIfFoundOne()
        {
                string csvFolderPath = ConfigurationManager.AppSettings["CsvFolderPath"];

                List<string> files = Directory.GetFiles(csvFolderPath, "*.*", SearchOption.TopDirectoryOnly).Where(n => n.Contains(".csv")).ToList();

                if (files.Count > 1)
                {
                    throw new Exception($"Error: {DateTime.Now}: There are more csv files in folder {csvFolderPath}. Should be 1");
                }
                else if (files.Count == 0)
                {
                    throw new Exception($"csv file was not found in folder {csvFolderPath}. No actions taken furthermore");
                }

                return files.First();
            
        }

        public void DeleteFirstLineWithDiyesSymbol(string scvFilePath)
        {
            try{
                var lines = File.ReadAllLines(scvFilePath).ToList();
                if (lines.First().First() == '#')
                {
                    lines.Remove(lines.First());
                }


                File.WriteAllLines(scvFilePath, lines);
            }catch( Exception ex)
            {
                throw new Exception($"Some error occured in method of deleting first line of file with #. Exception content: {ex.Message}", ex);
            }
        }

        public void CopyCsvfileInHistoryFolder(string csvFilePath)
        {
            try
            {
                string format = "ddMMyyyy-hhmmss";
                File.Copy(csvFilePath, $"{ConfigurationManager.AppSettings["CsvHistoryFilesFolderPath"]}/{Path.GetFileNameWithoutExtension(csvFilePath)}{DateTime.Now.ToString(format)}{Path.GetExtension(csvFilePath)}");
            }catch( Exception ex)
            {
                throw new Exception($"Coudn't copy file in history csv file forlder. Exception content: {ex.Message}", ex);
            }
        }

        public void DeleteCsvFileFromFolder(string csvFilePath)
        {
            try
            {
                File.Delete(csvFilePath);
            }catch(Exception ex)
            {
                throw new Exception($"Couldn't delete file from csv files folder. Exception content: {ex.Message}");
            }
        }
    }
}

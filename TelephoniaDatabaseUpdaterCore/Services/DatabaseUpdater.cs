using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Text;
using TelephoniaDatabaseUpdaterCore.Models;

namespace TelephoniaDatabaseUpdaterCore.Services
{
    class DatabaseUpdater
    {
        public void UpdateDatabase()
        {
            ILogger logger = new FileLogger();
            CsvFileService csvFileService = new CsvFileService();
            AsterixApiService asterixApiService = new AsterixApiService();
            SqlDataBaseService sqlDataBaseService = new SqlDataBaseService();
            CsvWorkdersService csvWorkersService = new CsvWorkdersService();

            try
            {
                string csvFilePath = csvFileService.GetCsvFileFromFolderIfFoundOne();
                try
                {
                    csvFileService.CopyCsvfileInHistoryFolder(csvFilePath);
                    csvFileService.DeleteFirstLineWithDiyesSymbol(csvFilePath);

                    List<AsterixWorker> asterixWorkers;
                    List<CsvWorker> csvWorkers;

                    try
                    {
                        asterixWorkers = asterixApiService.GetAsterixWorkersByApi(ConfigurationManager.AppSettings["AsterixUrl"]);
                        logger.Log($"Success: {DateTime.Now}: Successfully got data from asterix api");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"ERROR in getting asterix data\n\t {ex.Message}");

                    }
                    try
                    {
                        csvWorkers = csvWorkersService.GetCsvWorkers(csvFilePath);
                        logger.Log($"Success: {DateTime.Now} Successfully read csv file.");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"ERROR during reading csv file and preraring csvWorkers list\n\t:  {ex.Message}");

                    }

                    SQLiteTransaction sqlTransaction = null;
                    using (SQLiteConnection sqlConn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                    {

                        sqlConn.Open();
                        sqlTransaction = sqlConn.BeginTransaction();

                        try
                        {
                            sqlDataBaseService.ClearDatabase(sqlConn, sqlTransaction);
                            logger.Log($"Success: {DateTime.Now}: Successfully erased database data");
                        }
                        catch (Exception ex)
                        {
                            sqlDataBaseService.RollbackDatabase(sqlTransaction, new Exception($"ERROR during erasing database data\n\t  {ex.Message}", ex));
                        }


                        try
                        {
                            new SqlDataBaseService().FillDatabase(sqlConn, sqlTransaction, csvWorkers, asterixWorkers);
                            logger.Log($"Success: {DateTime.Now} Successfully filled data in database.");
                        }
                        catch (Exception ex)
                        {
                            sqlDataBaseService.RollbackDatabase(sqlTransaction, new Exception($"ERROR during writing data in database\n\t.  {ex.Message}"));
                        }

                        sqlTransaction.Commit();
                    }
                }finally
                {
                    csvFileService.DeleteCsvFileFromFolder(csvFilePath);
                }

            }catch( Exception ex)
            {
                
                logger.Log($"{DateTime.Now}\n\t {ex.Message}");
            }
            finally
            {
                
                logger.Log("\n\n");
            }

        }


    }
}

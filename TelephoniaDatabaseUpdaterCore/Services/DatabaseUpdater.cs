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
            try
            {
                SQLiteTransaction sqlTransaction = null;
                using (SQLiteConnection sqlConn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {

                    sqlConn.Open();
                    sqlTransaction = sqlConn.BeginTransaction();

                    List<AsterixWorker> asterixWorkers;
                    List<CsvWorker> csvWorkers;

                    ILogger logger = new FileLogger();
                    AsterixApiService asterixApiService = new AsterixApiService();
                    SqlDataBaseService sqlDataBaseService = new SqlDataBaseService();
                    CsvWorkdersService csvWorkersService = new CsvWorkdersService(new CsvFileSearcher());

                    try
                    {
                        asterixWorkers = asterixApiService.GetAsterixWorkersByApi(ConfigurationManager.AppSettings["AsterixUrl"]);
                        logger.Log($"Success: {DateTime.Now}: Successfully got data from asterix api");
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"ERROR in getting asterix data : {DateTime.Now}:  {ex.Message}");
                        throw;
                    }

                    try
                    {
                        csvWorkers = csvWorkersService.GetCsvWorkers();
                        logger.Log($"Success: {DateTime.Now} Successfully read csv file.");
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"ERROR during reading csv file and preraring csvWorkers list: {DateTime.Now}: {ex.Message}");
                        throw;
                    }

                    try
                    {
                        sqlDataBaseService.ClearDatabase(sqlConn, sqlTransaction);
                        logger.Log($"Success: {DateTime.Now}: Successfully erased database data");
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"ERROR during erasing database data: {DateTime.Now}: {ex.Message}");
                        sqlDataBaseService.RollbackDatabase(sqlTransaction, ex);
                    }


                    try
                    {
                        new SqlDataBaseService().FillDatabase(sqlConn, sqlTransaction, csvWorkers, asterixWorkers);
                        logger.Log($"Success: {DateTime.Now} Successfully filled data in database.");
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"ERROR during writing data in database: {DateTime.Now}: {ex.Message}");
                        sqlDataBaseService.RollbackDatabase(sqlTransaction, ex);
                    }

                    sqlTransaction.Commit();
                }
            }
            catch (Exception ex) { }
            finally
            {
                ILogger logger = new FileLogger();
                logger.Log("\n\n");
            }
        }

        
    }
}

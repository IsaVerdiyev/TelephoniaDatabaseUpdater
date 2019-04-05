using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using TelephoniaDatabaseUpdaterCore.Models;

namespace TelephoniaDatabaseUpdaterCore.Services
{
    class SqlDataBaseService
    {
        public void ClearDatabase(SQLiteConnection sQLiteConnection, SQLiteTransaction sQLiteTransaction)
        {
            string commandText = "Delete from PhoneUsersFull";
            SQLiteCommand sqlCommand = new SQLiteCommand(commandText, sQLiteConnection, sQLiteTransaction);
            sqlCommand.ExecuteNonQuery();
        }

        public void FillDatabase(SQLiteConnection sQLiteConnection, SQLiteTransaction sQLiteTransaction, List<CsvWorker> csvWorkers, List<AsterixWorker> asterixWorkers)
        {
            foreach (var csvWoker in csvWorkers)
            {
                var foundAsterixWorker = asterixWorkers.FirstOrDefault(w => w.phoneUserNumber.Contains(csvWoker.OfficePhone));
                if (foundAsterixWorker != null)
                {
                    string commandText = "insert into PhoneUsersFull " +
                        "(phoneUserDepartment, phoneUserEmail, phoneUserName, phoneUserNameAZE, phoneUserStructure, phoneUserSpecialty, phoneUserNumber) " +
                        "Values(@userDepartment, @userEmail, @userName, @userNameAze, @userStructure, @userSpecialty, @userNumber)";

                    SQLiteCommand sqlCommand = new SQLiteCommand(commandText, sQLiteConnection, sQLiteTransaction);
                    sqlCommand.Parameters.Add("@userDepartment", DbType.String);
                    sqlCommand.Parameters.Add("@userEmail", DbType.String);
                    sqlCommand.Parameters.Add("@userName", DbType.String);
                    sqlCommand.Parameters.Add("@userNameAze", DbType.String);
                    sqlCommand.Parameters.Add("@userStructure", DbType.String);
                    sqlCommand.Parameters.Add("@userSpecialty", DbType.String);
                    sqlCommand.Parameters.Add("@userNumber", DbType.String);


                    sqlCommand.Parameters["@userDepartment"].Value = csvWoker.Department;
                    sqlCommand.Parameters["@userEmail"].Value = csvWoker.Mail;
                    sqlCommand.Parameters["@userName"].Value = foundAsterixWorker.phoneUserName;
                    sqlCommand.Parameters["@userNameAze"].Value = csvWoker.DisplayName;
                    sqlCommand.Parameters["@userStructure"].Value = csvWoker.Office;
                    sqlCommand.Parameters["@userSpecialty"].Value = csvWoker.Title;
                    sqlCommand.Parameters["@userNumber"].Value = foundAsterixWorker.phoneUserNumber;
                    sqlCommand.Parameters["@userDepartment"].Value = csvWoker.Department;
                    sqlCommand.ExecuteNonQuery();

                }
            }
        }

        public void RollbackDatabase(SQLiteTransaction transaction, Exception ex)
        {
            try
            {
                transaction.Rollback();
                throw new Exception($"{ex.Message}.\n Database rolled back successfully");
            }
            catch (Exception rollbackException)
            {
                throw new Exception($"{ex.Message}.\nERROR during rollback of database. Exception content : {rollbackException.Message}");
            }
        }
    }
}

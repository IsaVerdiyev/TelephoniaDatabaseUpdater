using CsvHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using TelephoniaDatabaseUpdaterCore.Models;

namespace TelephoniaDatabaseUpdaterCore.Services
{
    class CsvWorkdersService
    {
        private readonly CsvFileSearcher csvFileSearcher;

        public CsvWorkdersService(CsvFileSearcher csvFileSearcher)
        {
            this.csvFileSearcher = csvFileSearcher;
        }

        public List<CsvWorker> GetCsvWorkers()
        {


            List<CsvWorker> csvWorkers = new List<CsvWorker>();
             
            using (var reader = new StreamReader(csvFileSearcher.GetCsvFileFromFolderIfFoundOne(), true))
            {
                using (var csv = new CsvReader(reader))
                {
                    var CswInitialWorkers = csv.GetRecords<CsvInitialWorker>();

                    foreach (var cswInitialWorker in CswInitialWorkers)
                    {
                        cswInitialWorker.officePhone = cswInitialWorker.officePhone.Trim();
                        if (string.IsNullOrEmpty(cswInitialWorker.officePhone))
                        {
                            continue;
                        }
                        List<string> phoneNumbers = cswInitialWorker.officePhone.Split(',').ToList();
                        for (int i = 0; i < phoneNumbers.Count(); i++)
                        {
                            phoneNumbers[i] = phoneNumbers[i].Trim();
                            if (string.IsNullOrEmpty(phoneNumbers[i]))
                            {
                                continue;
                            }

                            var cswWorker = new CsvWorker
                            {
                                Department = cswInitialWorker.department,
                                GivenName = cswInitialWorker.givenName,
                                DisplayName = cswInitialWorker.displayName,
                                Mail = cswInitialWorker.mail,
                                Office = cswInitialWorker.office,
                                OfficePhone = phoneNumbers[i],
                                Sn = cswInitialWorker.sn,
                                Title = cswInitialWorker.title
                            };
                            csvWorkers.Add(cswWorker);
                        }
                    }
                }
            }
            return csvWorkers;
        }
    }
}

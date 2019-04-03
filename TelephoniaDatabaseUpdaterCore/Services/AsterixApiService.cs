using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TelephoniaDatabaseUpdaterCore.Models;

namespace TelephoniaDatabaseUpdaterCore.Services
{
    class AsterixApiService
    {
        public List<AsterixWorker> GetAsterixWorkersByApi(string asterixUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage httpClientResponse = client.GetAsync(asterixUrl).Result;
                HttpContent content = httpClientResponse.Content;
                string result = content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<AsterixWorker>>(result);
            }
        }
    }
}

using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Lib.Contracts;
using Core.Lib.Models;
using Newtonsoft.Json;
using NLog;

namespace Core.Lib
{
    public class WatsonAiActivity : IAiActivity<WatsonTextAnalysisObject>
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public async Task<WatsonTextAnalysisObject> AnalyzeText(string term, HttpClient client)
        {
            try
            {
                string json;
                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/watson.json");
                using (var reader = new StreamReader(jsonPath))
                {
                    json = reader.ReadToEnd();
                }

                json = json.Replace("%content%", term);

                var apiKey = ConfigurationManager.AppSettings["WatsonKey"];
                var api = ConfigurationManager.AppSettings["WatsonNluApi"];

                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"apikey:{apiKey}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(api, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new WatsonTextAnalysisObject
                    {
                        WatsonTextAnalysisResponse = null,
                        Success = false
                    };
                }
                var body = response.Content.ReadAsStringAsync().Result;
                var aiResponse = JsonConvert.DeserializeObject<WatsonTextAnalysisResponse>(body);

                return new WatsonTextAnalysisObject
                {
                    WatsonTextAnalysisResponse = aiResponse,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }
    }
}
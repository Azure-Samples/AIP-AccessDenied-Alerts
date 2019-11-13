using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AIPAccessDeniedAlerts
{
    public class TIIndicatorsController
    {

        #region Create TI
        public async Task<string> CreateTI(TiIndicator tiIndicator, string token)
        {
            //var token = await GetToken();

            string url = string.Format("https://graph.microsoft.com/beta/security/tiIndicators");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var stringTIIndicator = JsonConvert.SerializeObject(tiIndicator);

            request.Content = new StringContent(stringTIIndicator, Encoding.UTF8, "application/json");

            HttpClient http = new HttpClient();

            var response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                return JsonConvert.SerializeObject(formatted, Formatting.Indented);
            }

            string json = await response.Content.ReadAsStringAsync();

            return json;
        }
        #endregion

    }
}
